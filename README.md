# Room Booking System - Technical Test

Aplikasi manajemen peminjaman ruangan berbasis ASP.NET Core Razor Pages yang dilengkapi dengan REST API Contract.

## 🚀 Cara Menjalankan Proyek
1. Pastikan Anda sudah menginstall **.NET SDK** (versi terbaru).
2. Sesuaikan *Connection String* database MySQL di file `appsettings.json`.
3. Jalankan migrasi database (jika diperlukan) atau pastikan database sudah terhubung.
4. Buka terminal di folder proyek, lalu jalankan perintah:
   ```bash
   dotnet run
#🔌 REST API Documentation (Blueprint Specification)
Seluruh endpoint API publik tersedia di bawah Base URL:
http://localhost:5150/api/v1

1. Master Data Ruangan
Endpoint: GET /api/v1/rooms
Deskripsi: Mengambil seluruh daftar ruangan yang tersedia di dalam sistem.
Response (200 OK):
JSON
`[
  {
    "id": 1,
    "name": "Ruang Meeting A",
    "capacity": 10,
    "location": "Lantai 2"
  }
]`
2. Dashboard Summary / Statistik
Endpoint: GET /api/v1/dashboard/summary
Deskripsi: Menampilkan ringkasan metrik statistik untuk dasbor utama.
Response (200 OK):
JSON
`{
  "totalRooms": 5,
  "totalBookings": 12,
  "pending": 2,
  "approved": 9,
  "rejected": 1
}`
3. Pengajuan Peminjaman Ruangan (Create Booking)
Endpoint: POST /api/v1/bookings
Deskripsi: Mengajukan jadwal peminjaman ruangan baru.
Request Headers: Content-Type: application/json
Request Body:
JSON
`{
  "roomId": 1,
  "userId": 1,
  "departmentId": 1,
  "title": "Rapat Koordinasi Proyek",
  "startTime": "2026-08-10T09:00:00",
  "endTime": "2026-08-10T11:00:00"
}`
Response (201 Created):

JSON
`{
  "message": "Peminjaman berhasil diajukan.",
  "bookingId": 15
}`
4. Eksekusi Status Booking (Approve / Reject)
Endpoint: PUT /api/v1/bookings/{id}/status
Deskripsi: Mengubah status pengajuan peminjaman (menyetujui atau menolak).
Request Headers: Content-Type: application/json
Request Body (Contoh Reject):

JSON
`{
  "status": "Rejected",
  "rejectionReason": "Ruangan sedang digunakan untuk acara kedinasan."
}`
Request Body (Contoh Approve)
JSON
`{
  "status": "Approved",
  "rejectionReason": null
}`
Response (200 OK):

JSON
`{
  "message": "Status peminjaman berhasil diperbarui."
}`

5. Jadwal Kalender (Calendar View Data)
Endpoint: GET /api/v1/bookings/calendar
Deskripsi: Mengambil daftar peminjaman yang hanya berstatus 'Approved' untuk ditampilkan pada antarmuka kalender.
Response (200 OK):
JSON
`[
  {
    "id": 12,
    "title": "Rapat Koordinasi Proyek",
    "start": "2026-08-10T09:00:00",
    "end": "2026-08-10T11:00:00",
    "roomName": "Ruang Meeting A"
  }
]`

6. Ekspor Laporan (Export Excel)
Endpoint: GET /api/v1/reports/export/excel
Deskripsi: Mengunduh berkas laporan rekapitulasi data peminjaman ruangan.
Response (200 OK / File Stream): Berkas berformat .xlsx siap diunduh

#🛠️ Tech Stack
Framework: ASP.NET Core (.NET 8 / Latest)
UI Layer: Razor Pages + Bootstrap
API Layer: ASP.NET Core Web API Controllers (/api/v1/...)
ORM: Entity Framework Core dengan MySQL (Pomelo Provider)
Reporting: QuestPDF
