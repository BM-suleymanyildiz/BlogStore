# 📚 BlogStore - Katmanlı ASP.NET Core Blog Projesi 🚀

BlogStore, **ASP.NET Core MVC**, **Entity Framework Core** ve **Hugging Face AI** entegrasyonu ile geliştirilmiş, katmanlı mimariye sahip, yapay zeka destekli modern bir blog uygulamasıdır. Kullanıcıların güvenli bir şekilde makale okuyup yorum yapabileceği, admin paneli üzerinden içerik yönetiminin sağlanabileceği fonksiyonel bir sistem sunar.

---

## 🚦 Kullanılan Katmanlı Mimari Yapı 🏗️

Proje, **4 Katmanlı Mimari** ile geliştirilmiştir. Bu yapı, uygulamanın sürdürülebilirliğini ve okunabilirliğini artırır.

### 1. Presentation (Sunum) Katmanı 🖥️
- ASP.NET Core MVC mimarisi üzerine kuruludur.
- `Controller`, `View` ve `ViewModel` katmanlarını içerir.
- Kullanıcı ile doğrudan etkileşim buradan yönetilir.
- `AJAX`, `Bootstrap 5` ve `jQuery` ile dinamik arayüz.

### 2. Entity (Varlık) Katmanı 📦
- Veritabanı tablolarını temsil eden sınıflar içerir: `AppUser`, `Article`, `Category`, `Tag`, `Comment`.
- Sadece veri yapıları ve ilişki tanımları bulunur.
- Hiçbir iş mantığı barındırmaz.

### 3. DataAccess (Veri Erişim) Katmanı 💾
- `Entity Framework Core` kullanılarak veritabanı işlemleri yapılır.
- `Generic Repository Pattern` uygulanmıştır.
- Tüm `CRUD` işlemleri ve özel sorgular bu katmanda tanımlanır.
- Örnek: `GetArticlesByCategoryIdAsync`, `GetLatestComments`, vb.

### 4. Business (İş) Katmanı ⚙️
- İş kuralları, servis sınıfları, validasyon işlemleri bu katmanda yer alır.
- `ToxicDetectionService` burada tanımlanmıştır.
- Servis bağımlılıkları `Extension` sınıfı aracılığıyla merkezi olarak yönetilir.
- `FluentValidation` ile girdi doğrulama işlemleri yapılır.

---

## 💡 Proje Özellikleri ve Fonksiyonlar

### 🧠 Yapay Zeka Destekli Yorum Denetleme (ToxicBERT)
- Hugging Face’in `toxic-bert` modeli ile zararlı yorumlar tespit edilir.
- Türkçe yorumlar, önce `Helsinki-NLP/opus-mt-tr-en` modeli ile İngilizce'ye çevrilir.
- AI tarafından 6 kategoriye göre analiz yapılır:  
  `toxic`, `insult`, `obscene`, `identity_hate`, `severe_toxic`, `threat`.
- Eğer herhangi bir kategori %5’in üzerinde sonuç verirse, yorum reddedilir.

### 🗃️ AJAX ile Dinamik Yorum Sistemi
- Sayfa yenilenmeden yorum ekleme / gösterme işlemleri.
- Giriş yapmayan kullanıcılar için yorum alanı gizlenir.
- Zararlı içerikli yorumlara anında yapay zeka filtresi uygulanır.

### 🔐 Identity ve Güvenlik
- `ASP.NET Core Identity` ile kullanıcı yönetimi.
- Roller: `Admin`, `Kullanıcı`, `Ziyaretçi`.
- Özellikler:
  - Parola sıfırlama
  - Giriş ve kayıt işlemleri
  - Yetki bazlı yönlendirmeler

### 🛡️ Güvenlik Önlemleri
- `CSRF` koruması (Tüm formlarda `AntiForgeryToken`)
- `Input Validation` (Sunucu taraflı doğrulamalar)
- `Authorization` (Rol bazlı erişim kontrolü)
- `Slug` kullanımı ile URL güvenliği  
  Örnek: `/articles/modern-yazilim-gelistirme`

---

## 🧩 Veritabanı Varlıkları ve İlişkiler

| Entity     | Açıklama                                      |
|------------|-----------------------------------------------|
| `AppUser`  | Sisteme kayıtlı kullanıcı bilgilerini tutar   |
| `Article`  | Makale başlığı, içeriği, görseli, yazarı      |
| `Category` | Makalelerin kategorilere göre ayrımını sağlar |
| `Tag`      | Makalelere etiket (keyword) eklemeyi sağlar   |
| `Comment`  | Makale altına yazılan kullanıcı yorumları     |

**İlişkiler:**
- Bir `Article`, bir `AppUser` tarafından yazılır.
- Bir `Article`, bir `Category` içinde yer alır.
- Bir `Article` birden fazla `Tag` alabilir.
- Bir `Comment`, bir `AppUser` tarafından yazılır ve bir `Article`'a aittir.

---

## 🧑‍💼 Kullanıcı Rolleri

| Rol       | Yetkiler                                                                 |
|-----------|--------------------------------------------------------------------------|
| Admin     | Tüm sistem yönetimi: kullanıcılar, içerikler, kategoriler, yorumlar      |
| Kullanıcı | Makale okuma, yorum yapma, profil bilgilerini düzenleme                  |
| Ziyaretçi | Sadece içerik görüntüleme; giriş yapılmadan yorum yazılamaz              |

---

## 🛠️ Sayfa ve Panel Özellikleri

### 🎛️ Admin Paneli
- **Dashboard:** Son 5 yorum, son 4 makale, kategori-makale dağılımı grafik
- **Makalelerim:** Admin'in yazdığı makaleler kart yapısında listelenir
- **Yeni Makale:** Başlık, içerik, görsel URL, kategori seçimi ile makale ekleme
- **Profilim:** Profil bilgisi düzenleme, değişiklik sonrası oturum sonlandırılır
- **Kategori ve Etiket Yönetimi:** CRUD işlemleri

### 🖥️ UI Paneli (Kullanıcı Arayüzü)
- **Ana Sayfa:** Yayında olan tüm makaleler listelenir
- **Makale Detay:** Makale içeriği, yazar bilgisi, yorumlar
- **Kategoriler:** Tüm kategoriler ve ilgili makaleler
- **Yazarlar:** Yazar profilleri ve yazdıkları içerikler
- **Giriş / Kayıt:** Identity UI ile güvenli kullanıcı girişi

---

## ⚙️ Kullanılan Teknolojiler

| Teknoloji             | Açıklama                               |
|-----------------------|----------------------------------------|
| ASP.NET Core 9.0      | MVC Web Uygulama çatısı                |
| Entity Framework Core | ORM aracı (Veritabanı işlemleri)       |
| SQL Server            | Veritabanı yönetim sistemi             |
| Bootstrap 5           | UI ve responsive tasarım               |
| Hugging Face API      | Yapay zeka servisi (toxic yorumlar)    |
| jQuery & AJAX         | Dinamik veri işlemleri                 |
| AutoMapper            | DTO - Entity dönüşüm yönetimi         |
| FluentValidation      | Veri doğrulama                         |

---

## 📷 Ekran Görüntüleri

<img width="1322" height="748" alt="Ekran görüntüsü 2025-08-02 221706" src="https://github.com/user-attachments/assets/2133c5b4-aaac-434f-9bb6-0941d37c10d3" />

<img width="733" height="877" alt="Ekran görüntüsü 2025-08-02 221713" src="https://github.com/user-attachments/assets/8450bcb4-71e0-4e70-982b-e85778fdeb7c" />

<img width="1895" height="829" alt="Ekran görüntüsü 2025-08-02 221330" src="https://github.com/user-attachments/assets/a130ebc3-e28d-403e-b8b2-b97935d1c4d9" />

<img width="1919" height="633" alt="Ekran görüntüsü 2025-08-02 221350" src="https://github.com/user-attachments/assets/3616fcce-178a-4729-abfb-64f7136ca123" />

<img width="1919" height="692" alt="Ekran görüntüsü 2025-08-02 221423" src="https://github.com/user-attachments/assets/6091eaca-9cf3-4584-8a2a-10abbf1ae7d9" />

<img width="1919" height="697" alt="Ekran görüntüsü 2025-08-02 221628" src="https://github.com/user-attachments/assets/d483966c-c6e8-4438-b00d-b807390d507a" />

<img width="1919" height="648" alt="Ekran görüntüsü 2025-08-02 221355" src="https://github.com/user-attachments/assets/2dcbfdc7-bee6-47b6-9861-51c0b024bc53" />

<img width="1919" height="556" alt="Ekran görüntüsü 2025-08-02 221430" src="https://github.com/user-attachments/assets/04146025-36bc-4673-b9e0-edc52494ba3c" />

<img width="1919" height="581" alt="Ekran görüntüsü 2025-08-02 221656" src="https://github.com/user-attachments/assets/14528c54-4c2a-40f2-94e5-1517a40eef76" />

<img width="1898" height="858" alt="Ekran görüntüsü 2025-08-02 221410" src="https://github.com/user-attachments/assets/c31976e7-87b8-4805-afa0-34925f646be5" />

<img width="1920" height="3225" alt="screencapture-localhost-7286-Default-Index-2025-08-02-22_00_58" src="https://github.com/user-attachments/assets/1a39acae-963c-452d-ab1f-9facd280f6c6" />

<img width="1007" height="1031" alt="Ekran görüntüsü 2025-08-02 221136" src="https://github.com/user-attachments/assets/f2eaf52f-6802-49d3-b4cf-f18e66dde5d3" />

<img width="605" height="488" alt="Ekran görüntüsü 2025-08-02 221228" src="https://github.com/user-attachments/assets/ac54b0bd-bafd-432b-b8ac-17af9523dded" />

<img width="598" height="529" alt="Ekran görüntüsü 2025-08-02 221234" src="https://github.com/user-attachments/assets/b65ce871-3129-432c-b2f8-384c56532084" />

<img width="1124" height="1236" alt="Ekran görüntüsü 2025-08-02 220250" src="https://github.com/user-attachments/assets/d66444a6-0c5e-49df-84ca-3fea1e8eaee4" />

<img width="1097" height="952" alt="Ekran görüntüsü 2025-08-02 220310" src="https://github.com/user-attachments/assets/33f067e0-092d-48c4-87de-4112e6ef4774" />

<img width="1347" height="1209" alt="Ekran görüntüsü 2025-08-02 220137" src="https://github.com/user-attachments/assets/bc1e072f-d357-447a-aadb-16cdc23fec50" />

<img width="1091" height="684" alt="Ekran görüntüsü 2025-08-02 220204" src="https://github.com/user-attachments/assets/7a2699d6-8b20-40b6-9046-56d090be0196" />





















---

