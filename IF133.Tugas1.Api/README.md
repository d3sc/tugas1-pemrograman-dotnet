# IF133 - Tugas 1

## Identitas

- **Nama:** Ikbar Rabbani
- **NIM:** 1003240029
- **Kelas:** IF semester 5

## Studi Kasus

Project ini menggunakan resource **Product** untuk mengelola data produk. Entity `Product` memiliki properti `Id`, `Name`, `Description`, `Price`, dan `Stock`. Data disimpan di database SQLite dan diakses menggunakan Entity Framework Core.

## Teknologi

- ASP.NET Core Web API (`net10.0`)
- Entity Framework Core `10.0.13`
- SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)

## Cara Menjalankan

Jalankan perintah berikut dari folder root repository:

1. Restore dependency:

   ```powershell
   dotnet restore .\IF133.Tugas1.Api
   ```

2. Terapkan migration ke database:

   ```powershell
   dotnet ef database update --project .\IF133.Tugas1.Api --startup-project .\IF133.Tugas1.Api
   ```

   Migration awal tersedia di `IF133.Tugas1.Api\Migrations`. Jika perintah `dotnet ef` belum tersedia, instal tool dengan versi yang sesuai:

   ```powershell
   dotnet tool install --global dotnet-ef --version 10.0.13
   ```

3. Jalankan API:

   ```powershell
   dotnet run --project .\IF133.Tugas1.Api
   ```

   Profil HTTP menggunakan `http://localhost:5222`; profil HTTPS juga menyediakan `https://localhost:7041`. Connection string `DefaultConnection` berada di `appsettings.json`, dan database SQLite bernama `tugas1.db` berada di folder project API.

## Endpoint

Base URL contoh: `http://localhost:5222`

| Method | Endpoint | Keterangan |
|---|---|---|
| GET | `/api/products` | Mengambil semua produk |
| GET | `/api/products/{id}` | Mengambil produk berdasarkan ID |
| POST | `/api/products` | Menambahkan produk |
| PUT | `/api/products/{id}` | Memperbarui produk |
| DELETE | `/api/products/{id}` | Menghapus produk |

Contoh body untuk POST dan PUT:

```json
{
  "name": "Buku Pemrograman .NET",
  "description": "Contoh produk",
  "price": 75000,
  "stock": 10
}
```

Respons yang diharapkan: GET berhasil `200 OK` (atau `404 Not Found` jika ID tidak ditemukan), POST `201 Created`, PUT `200 OK`, dan DELETE `204 No Content`. Field `name`, `price`, dan `stock` wajib diisi; harga dan stok tidak boleh negatif.

## Bukti Pengujian

Lengkapi tabel berikut setelah menjalankan pengujian melalui Postman atau HTTP client. Simpan screenshot di folder `Bukti` dan cantumkan nama berkasnya.

| Pengujian | Hasil yang diharapkan | Hasil aktual / bukti |
|---|---|---|
| POST `/api/products` | `201 Created`, produk tersimpan | Belum dilampirkan |
| GET `/api/products` | `200 OK`, daftar produk tampil | Belum dilampirkan |
| GET `/api/products/{id}` | `200 OK` untuk ID yang ada | Belum dilampirkan |
| PUT `/api/products/{id}` | `200 OK`, perubahan tersimpan | Belum dilampirkan |
| DELETE `/api/products/{id}` | `204 No Content` | Belum dilampirkan |
| GET setelah aplikasi dijalankan ulang | Data yang tersimpan tetap dapat dibaca | Belum dilampirkan |

Project menyediakan OpenAPI di `/openapi/v1.json` pada environment Development. Swagger UI tidak dikonfigurasi.

## Kendala dan Solusi

**Kendala:** API harus menggunakan database yang sama saat migration diterapkan dan saat endpoint dijalankan; jika connection string berbeda, perubahan data dapat masuk ke berkas database lain.

**Solusi:** Connection string `DefaultConnection` diletakkan di `appsettings.json` dan `AppDbContext` didaftarkan melalui dependency injection dengan provider SQLite. Jalankan perintah migration dan API dari konfigurasi project yang sama, lalu verifikasi persistence dengan membaca data kembali setelah aplikasi dijalankan ulang.