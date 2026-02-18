import { Route, Routes } from "react-router-dom"
import Layout from "./Layout"
import Store from "./Store"
import CartPage from "./CartPage"
import CheckoutPage from "./CheckoutPage"
import ProductDetail from "./ProductDetail"
import CheckoutSuccess from "./CheckoutSuccess"
import AdminLayout from "./AdminLayout"
import AdminDashboard from "./AdminDashboard"
import AdminProducts from "./AdminProducts"
import AdminLogin from "./AdminLogin"
import ProtectedRoute from "./ProtectedRoute"

function App() {
  return (
    <main className="font-body">
      <Routes>
        {/* Public Storefront */}
        <Route path="/" element={<Layout />}>
          <Route index element={<Store />} />
          <Route path="cart" element={<CartPage />} />
          <Route path="checkout" element={<CheckoutPage />} />
          <Route path="success" element={<CheckoutSuccess />} />
          <Route path="login" element={<AdminLogin />} />
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
