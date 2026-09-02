import React, { useEffect, useState } from 'react';
import Navbar from './components/Navbar';
import BookingTable from './components/BookingTable';
import BookingForm from './components/BookingForm';
import { resourceService } from './Api/resourceService';
export default function App() {
  const [resources, setResources] = useState([]);
  const [loadingResources, setLoadingResources] = useState(true);
  const [bookings, setBookings] = useState([]);  

   useEffect(() => {
    const fetchResources = async () => {
      try {
        const data = await resourceService.getAllResources();
        setResources(data);
      } catch (error) {
        console.error("Failed to load resources:", error);
      } finally {
        setLoadingResources(false);
      }
    };

    fetchResources();
  }, []);

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 p-8">
      <div className="max-w-6xl mx-auto grid grid-cols-1 md:grid-cols-1 gap-8">
         <BookingForm resources={resources} loadingResources={loadingResources} />
        <BookingTable resources={resources} loadingResources={loadingResources} bookings={bookings} />
      </div>
    </div>
  );
}