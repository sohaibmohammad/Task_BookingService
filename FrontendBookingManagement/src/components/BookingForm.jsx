import React, { useState, useEffect, useRef } from 'react';
import { bookingService } from '../Api/bookingService';

export default function BookingForm({ resources = [], loadingResources = false, onBookingCreated }) {
  const [formData, setFormData] = useState({
    resourceId: '',
    userId: '',  
    startDateTime: '',  
    endDateTime: ''  
  });
  
  const [isOpen, setIsOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [showAlert, setShowAlert] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const dropdownRef = useRef(null);

  useEffect(() => {
    if (resources.length > 0 && !formData.resourceId) {
      setFormData(prev => ({ ...prev, resourceId: resources[0].id }));
    }
  }, [resources]);

   useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const selectedResourceName = resources.find(r => r.id === formData.resourceId)?.name || "Select resource...";

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setShowAlert(false);
    setErrorMessage('');
    
    try {
      await bookingService.createBooking(formData);
      setShowAlert(true);
      
       setFormData(prev => ({
        ...prev,
        userId: '',
        startDateTime: '',
        endDateTime: ''
      }));

      if (onBookingCreated) {
        onBookingCreated();
      }
    } catch (error) {
      console.error("Failed to create booking:", error);
      const errorData = error.response?.data;
      if (typeof errorData === 'object' && errorData !== null) {
        const messages = Object.values(errorData.errors || errorData).flat().join(' | ');
        setErrorMessage(messages || errorData.message || "Failed to create booking.");
      } else {
        setErrorMessage("Failed to create booking. Please check availability.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="bg-slate-800 border border-slate-700 rounded-xl p-6 shadow-lg">
      <h2 className="text-lg font-semibold mb-6 text-slate-200">Create New Booking</h2>
      
      <form className="space-y-4" onSubmit={handleSubmit}>
        {/* Custom Dropdown */}
        <div className="relative" ref={dropdownRef}>
          <label className="block text-xs text-slate-400 mb-1">Resource</label>
          <button
            type="button"
            onClick={() => !loadingResources && setIsOpen(!isOpen)}
            className="w-full bg-slate-900 border border-slate-500 rounded-lg p-2.5 text-sm text-slate-200 flex justify-between items-center focus:outline-none focus:border-sky-500 text-left cursor-pointer"
          >
            <span className="truncate">{loadingResources ? "Loading resources..." : selectedResourceName}</span>
            <span className="text-slate-400 text-xs">▼</span>
          </button>

          {isOpen && (
            <ul className="absolute z-10 w-full mt-1 bg-slate-900 border border-slate-700 rounded-lg shadow-xl max-h-[200px] overflow-y-auto divide-y divide-slate-800">
              {resources.map((res) => (
                <li
                  key={res.id}
                  onClick={() => {
                    setFormData(prev => ({ ...prev, resourceId: res.id }));
                    setIsOpen(false);
                  }}
                  className={`p-2.5 text-sm cursor-pointer hover:bg-sky-600 hover:text-white transition ${
                    formData.resourceId === res.id ? 'bg-slate-800 text-sky-400' : 'text-slate-200'
                  }`}
                >
                  {res.name}
                </li>
              ))}
            </ul>
          )}
        </div>

        <div>
          <label className="block text-xs text-slate-400 mb-1">User ID / Name</label>
          <input 
            type="text" 
            value={formData.userId}
            onChange={(e) => setFormData(prev => ({ ...prev, userId: e.target.value }))}
            placeholder="e.g. user-guid-or-name"
            required
            className="w-full bg-slate-900 border border-slate-500 rounded-lg p-2.5 text-sm text-slate-200 focus:outline-none focus:border-sky-500"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-xs text-slate-400 mb-1">Start Time</label>
            <input 
              type="datetime-local" 
              value={formData.startDateTime}
              onChange={(e) => setFormData(prev => ({ ...prev, startDateTime: e.target.value }))}
              required
              className="w-full bg-slate-900 border border-slate-500 rounded-lg p-2.5 text-sm text-slate-200 focus:outline-none focus:border-sky-500"
            />
          </div>
          <div>
            <label className="block text-xs text-slate-400 mb-1">End Time</label>
            <input 
              type="datetime-local" 
              value={formData.endDateTime}
              onChange={(e) => setFormData(prev => ({ ...prev, endDateTime: e.target.value }))}
              required
              className="w-full bg-slate-900 border border-slate-500 rounded-lg p-2.5 text-sm text-slate-200 focus:outline-none focus:border-sky-500"
            />
          </div>
        </div>

        <button 
          type="submit" 
          disabled={loading}
          className="w-full mt-4 bg-sky-600 hover:bg-sky-500 text-white font-medium py-2.5 rounded-lg transition shadow-md cursor-pointer disabled:opacity-50"
        >
          {loading ? "Submitting..." : "Submit Booking"}
        </button>
      </form>

      {showAlert && (
        <div className="mt-4 p-3 bg-emerald-500/10 border border-emerald-500/30 rounded-lg flex items-center justify-between text-emerald-400 text-sm">
          <span>✓ Booking created successfully!</span>
          <button onClick={() => setShowAlert(false)} className="text-slate-400 hover:text-slate-200 text-xs cursor-pointer">✕</button>
        </div>
      )}

      {errorMessage && (
        <div className="mt-4 p-3 bg-red-500/10 border border-red-500/30 rounded-lg flex items-center justify-between text-red-400 text-sm">
          <span>{errorMessage}</span>
          <button onClick={() => setErrorMessage('')} className="text-slate-400 hover:text-slate-200 text-xs cursor-pointer">✕</button>
        </div>
      )}
    </section>
  );
}