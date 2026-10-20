# BBLift Client (MVC)

BBLift uygulamasının ASP.NET Core MVC arayüzüdür. Veriler için `BBLift` (API) deposuna bağlanır, önce API çalıştırılmalıdır.

## Yapı

- `BBLiftClient.Application`: API'ye istek atan servisler ve DTO'lar
- `BBLiftClient.MVC`: Controller'lar, view'lar, yönetim paneli (`Areas/Dashboard`)

## Sayfalar

Ana sayfa, egzersiz kütüphanesi, rutinler, antrenman başlatma ve takibi, geçmiş antrenmanlar, profil ve ayarlar, üyelik paketleri, sepet, ödeme, siparişler. Yönetim paneli (`/Dashboard`): egzersiz, kas grubu, ekipman, üyelik paketi ve kullanıcı rolleri (SuperAdmin).

## Çalıştırma

1. `BBLift` API'sini kurup çalıştırın (API deposundaki README).
2. `BBLiftClient.MVC/appsettings.json` içindeki `API:BaseUrl` değerinin API adresiyle (`https://localhost:7273/`) aynı olduğundan emin olun.
3. Projeyi çalıştırın:

```bash
cd BBLiftClient.MVC
dotnet run
```

MVC uygulaması `https://localhost:7247` adresinde açılır.

Yönetici girişi: `super.admin@bblift.com` / `Admin@123` (API ilk açılışta oluşturur).
