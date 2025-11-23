import { Route, Routes } from "react-router-dom"
import Layout from "./components/Layout"
import Store from "./pages/Store"
import CartPage from "./pages/CartPage"
import CheckoutPage from "./pages/CheckoutPage"
import ProductDetail from "./pages/ProductDetail"
import AdminLayout from "./components/admin/AdminLayout"
import AdminDashboard from "./pages/admin/AdminDashboard"
import AdminProducts from "./pages/admin/AdminProducts"
import AdminLogin from "./pages/admin/AdminLogin"
import ProtectedRoute from "./components/admin/ProtectedRoute"

function App() {
  return (
    <main className="font-body">
      <Routes>
        {/* Public Storefront */}
        <Route path="/" element={<Layout />}>
          <Route index element={<Store />} />
          <Route path="cart" element={<CartPage />} />
          <Route path="checkout" element={<CheckoutPage />} />
          <Route path="products/:id" element={<ProductDetail />} />
        </Route>

        {/* Admin Dashboard */}
        <Route path="/admin/login" element={<AdminLogin />} />
        <Route path="/admin" element={<ProtectedRoute><AdminLayout /></ProtectedRoute>}>
          <Route index element={<AdminDashboard />} />
          <Route path="products" element={<AdminProducts />} />
        </Route>
      </Routes>
    </main>
  )
}

export default App