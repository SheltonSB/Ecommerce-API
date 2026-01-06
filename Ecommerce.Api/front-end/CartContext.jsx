import React, { createContext, useContext, useState, useEffect } from 'react';
import { API_URL } from './config';

const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cart, setCart] = useState([]);

  // Helper to get headers with Auth token
  const getHeaders = () => {
    const token = localStorage.getItem('token');
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiration');
    setCart([]);
    window.location.href = '/';
  };

  useEffect(() => {
    const token = localStorage.getItem('token');
    const expiration = localStorage.getItem('tokenExpiration');

    if (token && expiration) {
      const timeLeft = parseInt(expiration, 10) - Date.now();
      if (timeLeft <= 0) {
        logout();
      } else {
        const timer = setTimeout(logout, timeLeft);
        return () => clearTimeout(timer);
      }
    }
  }, []);

  useEffect(() => {
    // Fetch the cart from the backend when the component mounts
    fetch(`${API_URL}/cart`, { headers: getHeaders() })
      .then((response) => {
        if (!response.ok) throw new Error('Failed to fetch cart');
        return response.json();
      })
      .then((data) => setCart(data))
      .catch((error) => console.error('Error fetching cart:', error));
  }, []);

  const addToCart = async (product) => {
    const previousCart = [...cart];
    // Optimistically update the UI
    setCart((prev) => [...prev, product]);

    try {
      const response = await fetch(`${API_URL}/cart`, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(product),
      });
      if (!response.ok) throw new Error('Failed to add to cart');

      // Update the local state with the actual saved item (containing DB ID)
      const savedItem = await response.json();
      setCart((prev) => prev.map((item) => (item === product ? savedItem : item)));
    } catch (error) {
      console.error('Error adding to cart:', error);
      // Revert to previous state if backend fails
      setCart(previousCart);
    }
  };

  const removeFromCart = async (indexToRemove) => {
    const previousCart = [...cart];
    const itemToRemove = cart[indexToRemove];
    setCart((prev) => prev.filter((_, index) => index !== indexToRemove));

    // Assuming the product object has an 'id' property
    if (itemToRemove && itemToRemove.id) {
      try {
        const response = await fetch(`${API_URL}/cart/${itemToRemove.id}`, {
          method: 'DELETE',
          headers: getHeaders(),
        });
        if (!response.ok) throw new Error('Failed to remove from cart');
      } catch (error) {
        console.error('Error removing from cart:', error);
        // Revert to previous state if backend fails
        setCart(previousCart);
      }
    }
  };

  const clearCart = async () => {
    const previousCart = [...cart];
    setCart([]);
    try {
      const response = await fetch(`${API_URL}/cart`, {
        method: 'DELETE',
        headers: getHeaders(),
      });
      if (!response.ok) throw new Error('Failed to clear cart');
    } catch (error) {
      console.error('Error clearing cart:', error);
      setCart(previousCart);
    }
  };

  const checkout = async () => {
    try {
      const response = await fetch(`${API_URL}/checkout`, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify({ items: cart }),
      });
      if (!response.ok) throw new Error('Checkout failed');
      setCart([]); // Clear cart on successful checkout
    } catch (error) {
      console.error('Error during checkout:', error);
      throw error; // Re-throw so the UI component can show an error message
    }
  };

  return (
    <CartContext.Provider value={{ cart, addToCart, removeFromCart, clearCart, checkout, logout }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => useContext(CartContext);