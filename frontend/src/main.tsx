import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import './index.css';
import { PageShell } from './components/ui/PageShell';
import { Products } from './pages/Products';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { VerifyEmail } from './pages/VerifyEmail';
import { Home } from './pages/Home';
import { AddProduct } from './pages/Admin/AddProduct';
import { ProductDetail } from './pages/ProductDetail';
import { Store } from './pages/Store';
import { Cart } from './pages/Cart';

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <BrowserRouter>
      <PageShell>
        <Routes>
          <Route path="/" element={<Navigate to="/store" replace />} />
          <Route path="/home" element={<Home />} />
          <Route path="/store" element={<Store />} />
          <Route path="/products" element={<Products />} />
          <Route path="/cart" element={<Cart />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/verify-email" element={<VerifyEmail />} />
          <Route path="/admin/products/new" element={<AddProduct />} />
          <Route path="/products/:id" element={<ProductDetail />} />
        </Routes>
      </PageShell>
    </BrowserRouter>
    <Toaster position="top-right" />
  </React.StrictMode>
);
