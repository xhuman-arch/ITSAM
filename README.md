# 🖥️ IT Service & Asset Management System

Aplikasi web untuk mengelola **aset IT** dan **layanan tiket (helpdesk)** dalam satu platform terintegrasi. Dibangun menggunakan **ASP.NET Core MVC** dengan database **MySQL**, dilengkapi fitur manajemen aset, tiket, permintaan penggantian barang, notifikasi, hingga audit log.

> ⚠️ **Catatan:** Project ini menggunakan ASP.NET Core (.NET) sehingga **tidak bisa di-deploy ke Vercel** (Vercel hanya mendukung Node.js/static site/serverless function terbatas). Saat ini project dijalankan secara **lokal** — belum ada demo live. Lihat bagian [Instalasi](#-instalasi--menjalankan-secara-lokal) untuk mencobanya sendiri.

---

## 📸 Screenshots

<!-- 
Tambahkan screenshot halaman utama di sini. 
Simpan gambar di folder `docs/screenshots/` lalu update link di bawah.
-->

| Dashboard | Asset Management |
|:---:|:---:|
| ![Dashboard](docs/screenshots/dashboard.png) | ![Asset](docs/screenshots/asset-index.png) |

| Ticket Management | Replacement Request |
|:---:|:---:|
| ![Ticket](docs/screenshots/ticket-index.png) | ![Replacement](docs/screenshots/replacement-request.png) |

---

## ✨ Fitur Utama

- **🔐 Autentikasi & Otorisasi** — Login/Register berbasis ASP.NET Core Identity dengan manajemen role/akses.
- **📊 Dashboard** — Ringkasan statistik aset, tiket, dan aktivitas terkini.
- **💻 Manajemen Aset (Asset)** — CRUD data aset IT (tambah, lihat detail, edit, hapus).
- **🎫 Manajemen Tiket (Ticket)** — Buat tiket, assign ke teknisi, tambah catatan troubleshooting, tracking status & SLA.
- **🔄 Replacement Request** — Pengajuan permintaan penggantian aset beserta proses approval-nya.
- **🔔 Notifikasi** — Notifikasi real-time terkait aktivitas tiket/aset.
- **📝 Audit Log** — Pencatatan histori perubahan data untuk keperluan tracking & keamanan.
- **📤 Export Data** — Export data ke Excel menggunakan ClosedXML.

---

## 🛠️ Tech Stack

| Kategori | Teknologi |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | MySQL (via Pomelo.EntityFrameworkCore.MySql) |
| ORM | Entity Framework Core |
| Autentikasi | ASP.NET Core Identity |
| Styling | Tailwind CSS |
| Export Excel | ClosedXML |

---

## 📁 Struktur Project

```
ITServiceAssetManagement/
├── Controllers/        # Logic controller (Asset, Ticket, Dashboard, dll)
├── Models/              # Entity & ViewModel
├── Views/               # Halaman Razor (.cshtml)
├── Services/            # Business logic (Notification, AuditLog, SLA, FileStorage)
├── wwwroot/              # Asset statis (CSS, JS, uploads)
├── appsettings.json     # Konfigurasi koneksi database
└── Program.cs           # Entry point aplikasi
```

---

## 🚀 Instalasi & Menjalankan Secara Lokal

### Prasyarat
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [Node.js](https://nodejs.org/) (untuk build Tailwind CSS)

### Langkah-langkah

1. **Clone repository**
   ```bash
   git clone https://github.com/username/ITServiceAssetManagement.git
   cd ITServiceAssetManagement
   ```

2. **Konfigurasi database**

   Edit `appsettings.json` sesuai koneksi MySQL kamu:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=itsam_db;User=root;Password=;TreatTinyAsBoolean=true;"
   }
   ```

3. **Jalankan migrasi database**
   ```bash
   dotnet ef database update
   ```

4. **Install dependency & build Tailwind CSS**
   ```bash
   npm install
   npm run build:css
   ```

5. **Jalankan aplikasi**
   ```bash
   dotnet run
   ```

6. Buka browser ke `https://localhost:5001` (atau port sesuai `launchSettings.json`).

> 💡 Untuk development CSS secara live, gunakan `npm run watch:css` di terminal terpisah.

---

## 🗺️ Roadmap

- [ ] Deploy ke hosting yang mendukung .NET (misal: Railway, Render, atau Azure App Service)
- [ ] Integrasi notifikasi email
- [ ] Role-based dashboard yang lebih granular

---

## 📄 Lisensi

Project ini dibuat untuk keperluan pembelajaran/portofolio. Bebas digunakan dan dimodifikasi.
