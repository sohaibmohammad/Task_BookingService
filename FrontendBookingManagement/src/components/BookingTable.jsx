import React, { useState, useEffect, useRef } from "react";
import { bookingService } from "../Api/bookingService";

export default function BookingTable({ resources, loadingResources }) {
  // 1. حالات الحقول المؤقتة (داخل الـ inputs)
  const [selectedResource, setSelectedResource] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [status, setStatus] = useState("");

  // 2. الفلاتر المعتمدة التي يتم جلب البيانات بناءً عليها فعلياً
  const [appliedFilters, setAppliedFilters] = useState({
    resourceId: "",
    startDate: "",
    endDate: "",
    status: "",
  });

  const [bookings, setBookings] = useState([]);
  const [loadingBookings, setLoadingBookings] = useState(false);

  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef(null);

  // تعيين أول مورد واعتراض الفلاتر أول مرة عند تحميل الموارد
  useEffect(() => {
    if (resources.length > 0 && !selectedResource) {
      const firstId = resources[0].id;
      setSelectedResource(firstId);
      setAppliedFilters({
        resourceId: firstId,
        startDate: "",
        endDate: "",
        status: "",
      });
    }
  }, [resources]);

  // جلب البيانات عند تغير الفلتر المعتمد أو رقم الصفحة فقط
  useEffect(() => {
    if (appliedFilters.resourceId) {
      fetchBookings();
    }
  }, [appliedFilters, pageNumber]);

  const fetchBookings = async () => {
    setLoadingBookings(true);
    try {
      const filters = {
        pageNumber,
        pageSize,
      };

      if (appliedFilters.startDate) filters.StartDate = appliedFilters.startDate;
      if (appliedFilters.endDate) filters.EndDate = appliedFilters.endDate;
      if (appliedFilters.status !== "") filters.Status = appliedFilters.status;

      const data = await bookingService.getBookingsByResource(
        appliedFilters.resourceId,
        filters
      );

      setBookings(data.items || []);
      setTotalPages(data.totalPages || Math.ceil((data.totalCount || 0) / pageSize) || 1);
      setTotalCount(data.totalCount || 0);
    } catch (error) {
      console.error("Failed to fetch resource bookings:", error);
      setBookings([]);
      setTotalPages(1);
      setTotalCount(0);
    } finally {
      setLoadingBookings(false);
    }
  };

  // عند الضغط على زر Apply Filter
  const handleApplyFilter = (e) => {
    e.preventDefault();
    setPageNumber(1); // العودة للصفحة الأولى عند تطبيق فلتر جديد
    setAppliedFilters({
      resourceId: selectedResource,
      startDate,
      endDate,
      status,
    });
  };

  const handleCancelBooking = async (bookingId) => {
    if (!window.confirm("Are you sure you want to delete this booking?"))
      return;

    try {
      await bookingService.cancelBooking(bookingId);
      setBookings((prevBookings) =>
        prevBookings.filter((b) => b.id !== bookingId),
      );
      setTotalCount((prev) => Math.max(0, prev - 1));
    } catch (error) {
      console.error("Failed to delete booking:", error);
      alert(error.response?.data?.message || "Failed to delete booking.");
    }
  };

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const selectedResourceName =
    resources.find((r) => r.id === selectedResource)?.name ||
    "Select resource...";

  const renderStatus = (st, isPast) => {
    // إذا كان التاريخ قديم (في الماضي)، نعرض الحالة كـ Completed بغض النظر عن قيمتها الأصلية
    if (isPast) {
      return (
        <span className="px-2 py-1 rounded text-xs bg-slate-700 text-slate-300 border border-slate-600">
          Completed
        </span>
      );
    }

    const isConfirmed = st === 1 || st === "Confirmed";
    const isCancelled = st === 2 || st === "Cancelled";
    const isCompleted = st === 3 || st === "Completed";

    if (isConfirmed) {
      return (
        <span className="px-2 py-1 rounded text-xs bg-emerald-500/10 text-emerald-400 border border-emerald-500/30">
          Confirmed
        </span>
      );
    } else if (isCancelled) {
      return (
        <span className="px-2 py-1 rounded text-xs bg-red-500/10 text-red-400 border border-red-500/30">
          Cancelled
        </span>
      );
    } else if (isCompleted) {
      return (
        <span className="px-2 py-1 rounded text-xs bg-slate-700 text-slate-300 border border-slate-600">
          Completed
        </span>
      );
    }
    return (
      <span className="px-2 py-1 rounded text-xs bg-slate-900 text-sky-400 border border-slate-700">
        {st}
      </span>
    );
  };

  const now = new Date();

  return (
    <section className="bg-slate-800 w-full border border-slate-700 rounded-xl p-6 shadow-lg flex flex-col justify-between">
      <div>
        <h2 className="text-lg font-semibold mb-4 text-slate-200">
          Bookings for Resource (Filtered)
        </h2>

        {/* نموذج الفلترة مع زر التطبيق */}
        <form onSubmit={handleApplyFilter} className="bg-slate-900/50 p-4 rounded-xl border border-slate-700 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
            
            {/* اختيار المورد */}
            <div className="relative w-full" ref={dropdownRef}>
              <label className="block text-xs text-slate-400 mb-1">Resource</label>
              <button
                type="button"
                onClick={() => !loadingResources && setIsOpen(!isOpen)}
                className="w-full bg-slate-900 border border-slate-700 rounded-lg p-2 text-sm text-slate-200 flex justify-between items-center focus:outline-none focus:border-sky-500 text-left cursor-pointer"
              >
                <span className="truncate">
                  {loadingResources ? "Loading..." : selectedResourceName}
                </span>
                <span className="text-slate-400 text-xs">▼</span>
              </button>

              {isOpen && (
                <ul className="absolute z-10 w-full mt-1 bg-slate-900 border border-slate-700 rounded-lg shadow-xl max-h-[200px] overflow-y-auto divide-y divide-slate-800">
                  {resources.map((res) => (
                    <li
                      key={res.id}
                      onClick={() => {
                        setSelectedResource(res.id);
                        setIsOpen(false);
                      }}
                      className={`p-2 text-sm cursor-pointer hover:bg-sky-600 hover:text-white transition ${
                        selectedResource === res.id
                          ? "bg-slate-800 text-sky-400"
                          : "text-slate-200"
                      }`}
                    >
                      {res.name}
                    </li>
                  ))}
                </ul>
              )}
            </div>

            {/* تاريخ البدء (Start Date) */}
            <div>
              <label className="block text-xs text-slate-400 mb-1">Start Date</label>
              <input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="w-full bg-slate-900 border border-slate-700 rounded-lg p-2 text-sm text-slate-200 focus:outline-none focus:border-sky-500"
              />
            </div>

            {/* تاريخ الانتهاء (End Date) */}
            <div>
              <label className="block text-xs text-slate-400 mb-1">End Date</label>
              <input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                className="w-full bg-slate-900 border border-slate-700 rounded-lg p-2 text-sm text-slate-200 focus:outline-none focus:border-sky-500"
              />
            </div>

            {/* فلتر الحالة (Status) */}
            <div>
              <label className="block text-xs text-slate-400 mb-1">Status</label>
              <select
                value={status}
                onChange={(e) => setStatus(e.target.value)}
                className="w-full bg-slate-900 border border-slate-700 rounded-lg p-2 text-sm text-slate-200 focus:outline-none focus:border-sky-500 cursor-pointer"
              >
                <option value="">All Statuses</option>
                <option value="1">Confirmed</option>
                <option value="2">Cancelled</option>
               </select>
            </div>

          </div>

          {/* زر التطبيق (Filter Button) */}
          <div className="flex justify-end">
            <button
              type="submit"
              className="bg-sky-600 hover:bg-sky-500 text-white px-5 py-2 rounded-lg text-sm font-semibold transition cursor-pointer"
            >
              Apply Filter
            </button>
          </div>
        </form>

        {/* الجدول */}
        <div className="overflow-x-auto mt-4">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-900 text-slate-400 uppercase text-xs">
              <tr>
                <th className="p-3">ID</th>
                <th className="p-3">Resource ID</th>
                <th className="p-3">User ID</th>
                <th className="p-3">Start Time</th>
                <th className="p-3">End Time</th>
                <th className="p-3">Status</th>
                <th className="p-3 text-center">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700">
              {loadingBookings ? (
                <tr>
                  <td colSpan="7" className="p-4 text-center text-slate-400">
                    Loading bookings...
                  </td>
                </tr>
              ) : bookings && bookings.length > 0 ? (
                bookings.map((b) => {
                  const isConfirmed = b.status === 1 || b.status === "Confirmed";
                  const bStartDate = b.startDateTime ? new Date(b.startDateTime) : null;
                  
                  // التحقق مما إذا كان التاريخ في الماضي
                  const isPast = bStartDate ? bStartDate < now : false;
                  
                  // الحذف ممنوع إذا كان التاريخ في الماضي أو الحالة ليست Confirmed
                  const canDelete = isConfirmed && !isPast;

                  return (
                    <tr key={b.id} className="hover:bg-slate-700/50">
                      <td className="p-3 font-medium text-slate-300 truncate max-w-[100px]">
                        {b.id}
                      </td>
                      <td className="p-3 text-slate-200">{b.resourceId}</td>
                      <td className="p-3 text-slate-200">{b.userId}</td>
                      <td className="p-3 text-slate-300">
                        {b.startDateTime
                          ? new Date(b.startDateTime).toLocaleString()
                          : "N/A"}
                      </td>
                      <td className="p-3 text-slate-300">
                        {b.endDateTime
                          ? new Date(b.endDateTime).toLocaleString()
                          : "N/A"}
                      </td>
                      <td className="p-3">{renderStatus(b.status, isPast)}</td>
                      <td className="p-3 text-center">
                        {canDelete ? (
                          <button
                            onClick={() => handleCancelBooking(b.id)}
                            className="bg-red-500/20 text-red-400 hover:bg-red-500 hover:text-white px-3 py-1 rounded text-xs font-semibold transition cursor-pointer"
                          >
                            Delete
                          </button>
                        ) : (
                          <span className="text-slate-500 text-xs">-</span>
                        )}
                      </td>
                    </tr>
                  );
                })
              ) : (
                <tr>
                  <td colSpan="7" className="p-4 text-center text-slate-400">
                    No bookings found matching the selected filters.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* أزرار التنقل بين الصفحات (Pagination) */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between mt-6 pt-4 border-t border-slate-700 text-sm text-slate-300">
            <div>
              Showing page{" "}
              <span className="font-semibold text-sky-400">{pageNumber}</span>{" "}
              of <span className="font-semibold">{totalPages}</span> (Total:{" "}
              {totalCount})
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
                disabled={pageNumber === 1}
                className="px-3 py-1.5 bg-slate-900 border border-slate-700 rounded-lg hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed transition cursor-pointer"
              >
                Previous
              </button>
              <button
                onClick={() =>
                  setPageNumber((prev) => Math.min(prev + 1, totalPages))
                }
                disabled={pageNumber === totalPages}
                className="px-3 py-1.5 bg-slate-900 border border-slate-700 rounded-lg hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed transition cursor-pointer"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>
    </section>
  );
}