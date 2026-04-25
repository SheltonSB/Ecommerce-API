# 🚀 How to Run the E-commerce API

## ✅ **Status: Fully Tested & Working**
- ✅ Build: Clean (0 errors)
- ✅ Tests: 51/51 passing (100%)
- ✅ Database: Configured & ready

---

## 🎯 **Quick Start (3 Simple Steps)**

### **Step 1: Open Terminal**
Open PowerShell or Command Prompt in the project root folder

### **Step 2: Run the API**
Execute one of these commands:

**Option A - Using batch file (Easiest):**
```bash
run-api.bat
```

**Option B - Using dotnet CLI:**
```bash
cd Ecommerce.Api
dotnet run
```

**Option C - Using PowerShell script:**
```powershell
.\Start-API.ps1
```

### **Step 3: Access the API**
Open your browser and go to:
```
http://localhost:5154
```

You'll see the Swagger UI with all API endpoints!

---

## 🧪 **To Run Tests**

**Option A - Using batch file:**
```bash
run-tests.bat
```

**Option B - Using dotnet CLI:**
```bash
dotnet test
```

**Expected Result:**
```
Test Run Successful.
Total tests: 51
     Passed: 51
 Total time: ~2 seconds
```

---

## 📊 **What You'll See When Running**

### **Console Output:**
```
Building...
Build succeeded.
[18:21:43 INF] Database migration and seeding completed successfully
[18:21:43 INF] E-commerce API is starting up...
[18:21:43 INF] Now listening on: http://localhost:5154
[18:21:43 INF] Application started. Press Ctrl+C to shut down.
```

### **Swagger UI:**
When you open http://localhost:5154 in your browser, you'll see:

- **Categories** section with 7 endpoints
- **Products** section with 11 endpoints  
- **Sales** section with 9 endpoints

Each endpoint has a "Try it out" button to test it!

---

## 🎯 **Quick API Tests in Swagger**

### **Test 1: Get All Categories**
1. Click on `GET /api/categories`
2. Click "Try it out"
3. Click "Execute"
4. You should see 4 categories

### **Test 2: Get All Products**
1. Click on `GET /api/products`
2. Click "Try it out"
3. Click "Execute"
4. You should see 4 products

### **Test 3: Create a New Product**
1. Click on `POST /api/products`
2. Click "Try it out"
3. Use this JSON:
```json
{
  "name": "Test Product",
  "description": "My test product",
  "price": 99.99,
  "sku": "TEST-123",
  "stockQuantity": 50,
  "isActive": true,
  "categoryId": 1
}
```
4. Click "Execute"
5. You should see a 201 Created response!

---

## 🔧 **Common Commands**

### **Build the Project**
```bash
dotnet build
```

### **Clean Build**
```bash
dotnet clean
dotnet build
```

### **Run Tests**
```bash
dotnet test
```

### **Run Tests with Details**
```bash
dotnet test --verbosity normal
```

### **Create Migration**
```bash
cd Ecommerce.Api
dotnet ef migrations add MigrationName
```

### **Apply Migration**
```bash
dotnet ef database update
```

### **Remove Last Migration**
```bash
dotnet ef migrations remove
```

---

## 📁 **Important Files & Locations**

| File/Folder | Description |
|-------------|-------------|
| `Ecommerce.Api/` | Main API project |
| `Ecommerce.Tests/` | Test project |
| `run-api.bat` | Quick start script |
| `run-tests.bat` | Quick test script |
| `README.md` | Complete documentation |
| `PROJECT_SUMMARY.md` | Project overview |
| `Ecommerce.db` | SQLite database (auto-created) |
| `logs/` | Log files (auto-created) |

---

## 🌐 **API Endpoints Overview**

Once running, you can access:

- **Swagger UI**: http://localhost:5154
- **Health Check**: http://localhost:5154/health
- **Get Categories**: http://localhost:5154/api/categories
- **Get Products**: http://localhost:5154/api/products
- **Get Sales**: http://localhost:5154/api/sales

---

## ⚡ **Sample Data Included**

The API comes pre-seeded with:
- ✅ 4 Categories (Electronics, Clothing, Books, Home & Garden)
- ✅ 4 Products (Smartphone, Laptop, T-Shirt, Programming Book)
- ✅ 16+ more products via Seed.cs (added at runtime)
- ✅ 20 Sample sales with various statuses
- ✅ Price history entries

---

## 🎯 **Testing the API**

### **Method 1: Swagger UI (Recommended)**
1. Run the API
2. Open http://localhost:5154
3. Use the "Try it out" buttons
4. See responses in real-time

### **Method 2: Postman**
1. Import `Ecommerce-API-Postman-Collection.json`
2. Set base URL to `http://localhost:5154`
3. Run any request

### **Method 3: cURL**
```bash
# Get all categories
curl http://localhost:5154/api/categories

# Get all products
curl http://localhost:5154/api/products

# Create a category
curl -X POST http://localhost:5154/api/categories `
  -H "Content-Type: application/json" `
  -d '{"name":"New Category","description":"Test"}'
```

---

## 🐛 **Troubleshooting**

### **Issue: Port already in use**
```powershell
# Find and kill process on port 5154
Get-Process -Id (Get-NetTCPConnection -LocalPort 5154).OwningProcess | Stop-Process
```

### **Issue: Database errors**
```bash
# Delete and recreate database
Remove-Item Ecommerce.Api/Ecommerce.db
cd Ecommerce.Api
dotnet ef database update
```

### **Issue: Build errors**
```bash
dotnet clean
dotnet restore
dotnet build
```

### **Issue: Migration errors**
```bash
cd Ecommerce.Api
dotnet ef migrations remove --force
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🎓 **First Time Setup**

If this is your first time running the project:

1. **Ensure .NET 9 SDK is installed**
   ```bash
   dotnet --version
   ```
   Should show 9.x.x

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build solution**
   ```bash
   dotnet build
   ```

4. **Run tests to verify**
   ```bash
   dotnet test
   ```

5. **Start the API**
   ```bash
   cd Ecommerce.Api
   dotnet run
   ```

---

## 🎉 **You're All Set!**

Your E-commerce API is now running and ready to showcase!

**Next Steps:**
1. ✅ Explore the Swagger UI
2. ✅ Test the endpoints
3. ✅ Review the code
4. ✅ Run the tests
5. ✅ Add to your portfolio!

---

## 📞 **Quick Reference**

| Command | Purpose |
|---------|---------|
| `dotnet build` | Build the project |
| `dotnet run` | Run the API |
| `dotnet test` | Run all tests |
| `dotnet clean` | Clean build artifacts |
| `run-api.bat` | Quick start (Windows) |

---

**Your API is production-ready! 🚀**

**Status**: 🟢 **READY TO RUN**








