# 🍎 MyStore — Apple-products E-Commerce

Full-stack storefront inspired by the Apple Store: clean, minimal UI built with vanilla
HTML / CSS / JavaScript on the frontend and an **ASP.NET Core 8 Web API** backed by
**SQL Server** on the backend. Real Apple product imagery, JWT auth, cart + checkout
flow, and a Stripe-ready (currently stubbed) payment service that you can swap in once
your keys are ready.

> **Languages**: README is in English; an Arabic walkthrough is at the bottom — [اذهب إلى الشرح بالعربية](#-تشغيل-المشروع-بالعربية).

---

## ✨ Features

| Frontend | Backend |
|---|---|
| Clean Apple-inspired design | ASP.NET Core 8 Web API |
| Responsive (mobile + desktop) | Layered architecture (Controllers → Services → Repositories) |
| Home with featured products + categories | EF Core 8 with SQL Server |
| Listing with filters (category, price, search) and sorting | JWT authentication & role-based authorization |
| Product detail page with quantity selector | BCrypt password hashing |
| Cart (server-side for logged-in users, local for anonymous) | CRUD: Products, Categories, Orders, Users, Cart |
| Checkout flow with order confirmation | Stripe-shaped payment service (stubbed; ready to swap) |
| Login / Register pages | Centralised exception middleware |
| Smooth fade/scale animations | Auto-migrate + seed real Apple sample data on startup |
| Order history page | Swagger UI for API exploration |

---

## 📁 Project Structure

```
mystore/
├── backend/
│   ├── MyStore.sln
│   └── MyStore.Api/
│       ├── Controllers/         # AuthController, ProductsController, CategoriesController,
│       │                        # CartController, OrdersController, UsersController
│       ├── Models/
│       │   ├── Entities/        # User, Product, Category, Order, OrderItem, CartItem
│       │   └── Dtos/            # Request/response DTOs (DTO suffix per role)
│       ├── Services/            # AuthService, ProductService, CartService, OrderService,
│       │                        # CategoryService, TokenService, StubPaymentService
│       ├── Repositories/        # Generic + specific repositories
│       ├── Data/                # AppDbContext, DbSeeder
│       ├── Migrations/          # EF Core migrations (InitialCreate)
│       ├── Middleware/          # ExceptionMiddleware
│       ├── Program.cs           # DI, JWT, Swagger, CORS, auto-migrate
│       └── appsettings.json     # Connection string + JWT + Stripe config
└── frontend/
    ├── index.html               # Home (hero + featured + categories)
    ├── products.html            # Listing with filters/sort/search
    ├── product.html             # Product detail
    ├── cart.html                # Bag
    ├── checkout.html            # Checkout flow
    ├── login.html / register.html
    ├── orders.html              # Order history
    ├── css/styles.css           # Apple-inspired design system
    └── js/
        ├── config.js            # API base URL — edit to point at your backend
        ├── api.js               # Fetch wrapper + auth token store
        ├── ui.js                # Shared navbar/footer/toast/formatting helpers
        ├── local-cart.js        # Anonymous cart that syncs on login
        └── pages/               # One JS module per page
```

---

## 🚀 Quick Start (Windows + LocalDB)

### Prerequisites
1. **Windows 10/11** (LocalDB is Windows-only)
2. **.NET 8 SDK** — <https://dotnet.microsoft.com/download/dotnet/8.0>
3. **SQL Server LocalDB** (ships with Visual Studio, or [download separately](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb))
4. Any modern browser

### 1) Clone and restore
```bash
git clone https://github.com/hassanramadan06/mystore.git
cd mystore
```

### 2) Run the API
```bash
cd backend/MyStore.Api
dotnet restore
dotnet run
```

The API listens on **`http://localhost:5080`** (configured in `Properties/launchSettings.json`).

On first run, EF Core will:
- Create the `MyStoreDb` database in LocalDB
- Apply the `InitialCreate` migration (creates Users, Categories, Products, Orders, OrderItems, CartItems tables)
- Seed real Apple sample data (4 categories, 14 products) and an admin user

> **Default admin account**:
> Email: `admin@mystore.local`  ·  Password: `Admin#123`
> Change this password (or remove the seeded admin) before deploying to production.

Open **`http://localhost:5080/swagger`** to explore every endpoint.

### 3) Run the frontend
The frontend is plain HTML/CSS/JS — any static file server works:

```bash
cd frontend
# Option A — Python 3
python -m http.server 5500
# Option B — VS Code Live Server extension (default port 5500)
```

Open **`http://localhost:5500`** and you're shopping.

> ⚠️ Do NOT open the `.html` files via the `file://` protocol — browsers block `fetch()`
> in that mode. Always serve them over HTTP.

### 4) Configure the API base URL
Edit [`frontend/js/config.js`](frontend/js/config.js) if your API runs on a different host/port:
```js
window.MYSTORE_CONFIG = {
  apiBase: "http://localhost:5080"
};
```

---

## 🐳 Alternative: SQL Server in Docker (any OS)

If you're on macOS/Linux or just prefer Docker, drop LocalDB and run:

```bash
docker run -d --name mystore-sql \
  -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Pass1" \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

Then in `backend/MyStore.Api/appsettings.Development.json` change the connection string to:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=MyStoreDb;User Id=sa;Password=YourStrong@Pass1;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

…and `dotnet run` exactly as before.

---

## 🔐 Authentication

- **POST `/api/auth/register`** — create a Customer account, returns `{ token, user }`
- **POST `/api/auth/login`** — issue a JWT, returns `{ token, user }`
- All authenticated endpoints expect: `Authorization: Bearer <token>`
- Roles: `Customer` (default) and `Admin` (seeded admin only).
  Admin-only endpoints: `POST/PUT/DELETE /api/products`, `POST/PUT/DELETE /api/categories`,
  `GET /api/orders/all`, `PUT /api/orders/{id}/status`, `GET /api/users`.

---

## 💳 Payments (Stripe)

The current `IPaymentService` implementation is **`StubPaymentService`** — it returns
deterministic fake `pi_stub_*` ids so the full checkout flow can be tested without a
Stripe account. To swap in real Stripe:

1. `dotnet add package Stripe.net` in `MyStore.Api/`
2. Implement `StripePaymentService : IPaymentService` using `PaymentIntentService`
3. Replace the registration in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IPaymentService, StripePaymentService>();
   ```
4. Add your test keys to `appsettings.Development.json` or user-secrets:
   ```json
   "Stripe": {
     "PublishableKey": "pk_test_...",
     "SecretKey": "sk_test_..."
   }
   ```
5. On the frontend, replace the confirmation screen in
   `frontend/js/pages/checkout.js` with Stripe.js + `confirmCardPayment(clientSecret)`.

---

## 📚 API Reference (highlights)

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | `/health` | — | Liveness check |
| POST | `/api/auth/register` | — | Register a new customer |
| POST | `/api/auth/login` | — | Issue a JWT |
| GET | `/api/categories` | — | List categories with product counts |
| GET | `/api/products` | — | Search/filter/sort/paginate. Query: `search`, `category`, `minPrice`, `maxPrice`, `sort`, `featured`, `page`, `pageSize` |
| GET | `/api/products/featured?take=8` | — | Featured products |
| GET | `/api/products/{id}` | — | Product detail |
| GET | `/api/cart` | Customer | Get the current user's cart |
| POST | `/api/cart/items` | Customer | Add to cart |
| PUT | `/api/cart/items/{productId}` | Customer | Update quantity (0 removes) |
| DELETE | `/api/cart/items/{productId}` | Customer | Remove item |
| DELETE | `/api/cart` | Customer | Empty the cart |
| POST | `/api/orders/checkout` | Customer | Create an order from cart (or explicit items), reserves stock, creates payment intent |
| GET | `/api/orders` | Customer | My orders |
| GET | `/api/orders/{id}` | Customer | Order detail (own only) |
| GET | `/api/orders/all` | Admin | All orders |
| PUT | `/api/orders/{id}/status` | Admin | Update order status |
| GET | `/api/users/me` | Customer | My profile |

---

## 🧪 Smoke test the API end-to-end

```bash
# 1. Register
curl -X POST http://localhost:5080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Hassan","email":"hassan@example.com","password":"pass1234"}'

# 2. Use the token
TOKEN="<paste-token-from-step-1>"

# 3. Add to cart
curl -X POST http://localhost:5080/api/cart/items \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":1}'

# 4. Checkout
curl -X POST http://localhost:5080/api/orders/checkout \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"shippingAddress":"123 Apple Ln","city":"Cupertino","postalCode":"95014","country":"USA"}'
```

---

## 🏗️ Architecture

```
HTTP request
   │
   ▼
Controllers      (thin — bind input, call services, format response)
   │
   ▼
Services         (business logic, transactions, validation)
   │
   ▼
Repositories     (data access — IRepository<T> + specific repos)
   │
   ▼
EF Core / AppDbContext
   │
   ▼
SQL Server
```

- **DI**: Everything is registered in `Program.cs` with `Scoped` lifetime.
- **Validation**: DTOs use `System.ComponentModel.DataAnnotations`; ASP.NET model binding rejects malformed input with `400`.
- **Errors**: `ExceptionMiddleware` translates `UnauthorizedAccessException → 401`,
  `InvalidOperationException → 400`, `KeyNotFoundException → 404`, anything else → `500`
  with the message logged.
- **Auth**: `TokenService` issues HS256 JWTs whose `sub` claim is the user id; the
  `[Authorize(Roles = "Admin")]` attribute gates admin endpoints.
- **CORS**: Configurable via `Cors:AllowedOrigins` in `appsettings.json`. The static
  frontend origin (`http://localhost:5500`) is allowed by default.

---

## 🗄️ Database Schema

```
Users (1) ───< Orders (M) ───< OrderItems (M) >─── Products (1)
   │                                                  │
   └───< CartItems (M) >─── Products (1) >── (M) Categories (1)
```

- **Users** — Id, FullName, Email *(unique)*, PasswordHash, Role (Customer/Admin), CreatedAt
- **Categories** — Id, Name, Slug *(unique)*, Description
- **Products** — Id, Name, Description, Price, Stock, ImageUrl, Brand, IsFeatured, CategoryId *(FK)*, CreatedAt
- **Orders** — Id, UserId *(FK)*, Total, Status (enum), ShippingAddress, City, PostalCode, Country, PaymentIntentId, CreatedAt
- **OrderItems** — Id, OrderId *(FK)*, ProductId *(FK)*, Quantity, UnitPrice
- **CartItems** — Id, UserId *(FK)*, ProductId *(FK)*, Quantity, UpdatedAt — unique on `(UserId, ProductId)`

---

## 🛠️ Common Commands

```bash
# Build
cd backend/MyStore.Api && dotnet build

# Add a new migration
dotnet ef migrations add AddSomething

# Apply migrations manually (not needed — done on startup by default)
dotnet ef database update

# Run with a custom connection string (one-shot override)
ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=MyStoreDb;User Id=sa;Password=YourStrong@Pass1;TrustServerCertificate=True" \
  dotnet run
```

---

## 🇪🇬 تشغيل المشروع بالعربية

### المتطلبات
- ويندوز 10/11
- .NET 8 SDK من <https://dotnet.microsoft.com/download/dotnet/8.0>
- SQL Server LocalDB أو Express
- متصفح حديث

### خطوات التشغيل
1. **استنسخ المشروع**:
   ```bash
   git clone https://github.com/hassanramadan06/mystore.git
   cd mystore
   ```

2. **شغّل الـ API**:
   ```bash
   cd backend/MyStore.Api
   dotnet restore
   dotnet run
   ```
   الـ API هيشتغل على `http://localhost:5080` وأول مرة هيعمل قاعدة البيانات تلقائياً
   ويحط فيها بيانات منتجات Apple حقيقية + يوزر أدمن:
   - **الإيميل**: `admin@mystore.local`
   - **الباسورد**: `Admin#123`

3. **افتح الـ Swagger** عشان تجرب كل الـ endpoints:
   `http://localhost:5080/swagger`

4. **شغّل الواجهة**:
   ```bash
   cd ../../frontend
   python -m http.server 5500
   ```
   وادخل على `http://localhost:5500` من أي متصفح.

5. **عدّل الـ API URL** (لو غيرت البورت):
   `frontend/js/config.js`

### لو الـ LocalDB مش متوفر (مثلاً على Mac/Linux)
استخدم **SQL Server في Docker** بدل LocalDB:
```bash
docker run -d --name mystore-sql \
  -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Pass1" \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```
وغيّر الـ `ConnectionStrings:DefaultConnection` في `appsettings.Development.json` للسطر:
```
Server=localhost,1433;Database=MyStoreDb;User Id=sa;Password=YourStrong@Pass1;TrustServerCertificate=True
```

### المميزات الرئيسية
- صفحة رئيسية مع منتجات مميّزة وفئات
- صفحة منتجات مع فلاتر (تصنيف، سعر، بحث، ترتيب)
- صفحة تفاصيل المنتج
- سلة تسوّق وصفحة دفع كاملة
- تسجيل دخول وإنشاء حساب (JWT)
- صفحة الطلبات السابقة
- تصميم Apple-style كامل ومتجاوب مع الموبايل

---

## 🔒 Production Hardening Checklist

Before deploying:

- [ ] Replace the dev `Jwt:Key` with a long random secret (≥32 bytes), stored in Key Vault / env vars
- [ ] Change or remove the seeded admin password
- [ ] Disable `Database:AutoMigrateAndSeed` in production and run migrations as a separate step
- [ ] Tighten the CORS allowed origins to your real frontend domain(s)
- [ ] Wire `Stripe.net` for real payments and add webhook handling (`/api/webhooks/stripe`)
- [ ] Enforce HTTPS (`UseHttpsRedirection`) and HSTS
- [ ] Add structured logging (Serilog/AppInsights) and request rate-limiting
- [ ] Move secrets out of `appsettings.json` (use user-secrets / env vars / Key Vault)

---

## 📜 License

MIT — feel free to use this as a starter for your own e-commerce project.
