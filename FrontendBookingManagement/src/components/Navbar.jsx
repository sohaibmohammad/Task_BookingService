import React from 'react';

export default function Navbar() {
  return (
    <header className="max-w-7xl mx-auto mb-8 bg-slate-800 border border-slate-700 rounded-xl p-4 shadow-lg flex items-center justify-between">
      <h1 className="text-xl font-bold text-sky-400">Booking Management System</h1>
      <span className="text-xs bg-slate-900 border border-slate-700 px-3 py-1.5 rounded-lg text-slate-300">
        Dashboard UI
      </span>
    </header>
  );
}