import api from "./axiosConfig"; // عدل المسار حسب مكان الـ axios instance عندك

export const bookingService = {
   createBooking: async (bookingData) => {
    const response = await api.post("/Bookings", bookingData);
    return response.data;
  },

   getBookingById: async (id) => {
    const response = await api.get(`/Bookings/${id}`);
    return response.data;
  },

   cancelBooking: async (id) => {
    const response = await api.put(`/Bookings/${id}/cancel`);
    return response.status;
  },

   getAllBookings: async (params = {}) => {
    console.log("yes");
    
    const response = await api.get("/Bookings", { params });

    console.log(response.data);
    
    
    return response.data;
  },

   getUserBookings: async (queryDto) => {
    const response = await api.get("/Bookings/user", { params: queryDto });
    return response.data;
  },

   checkAvailability: async (resourceId, startTime, endTime) => {
    const response = await api.get("/Bookings/check-availability", {
      params: { resourceId, startTime, endTime },
    });
    return response.data;
  },

   updateStatus: async (id, newStatus) => {
    const response = await api.patch(`/Bookings/${id}/status`, newStatus, {
      headers: { "Content-Type": "application/json" },
    });
    return response.data;
  },

   getBookedTimeSlots: async (resourceId, date) => {
    const response = await api.get("/Bookings/availability/slots", {
      params: { resourceId, date },
    });
    return response.data;
  },
};