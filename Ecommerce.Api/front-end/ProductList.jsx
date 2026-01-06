import React, { useEffect, useState } from 'react';
import { useCart } from './CartContext';
import { API_URL } from './config';

const ProductList = () => {
  const [products, setProducts] = useState([]);
  const [showAccountInfo, setShowAccountInfo] = useState(false);
  const { addToCart, logout } = useCart();
  const userEmail = localStorage.getItem('userEmail') || 'Guest';

  useEffect(() => {
    // Fetch real products from the backend
    fetch(`${API_URL}/products`)
      .then((res) => res.json())
      .then((data) => setProducts(data))
      .catch((err) => console.error('Error fetching products:', err));
  }, []);

  return (
    <div className="product-list">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>Our Products</h2>
        <div style={{ display: 'flex', gap: '10px' }}>
          <button onClick={() => setShowAccountInfo(!showAccountInfo)} style={{ backgroundColor: '#007bff', color: 'white' }}>
            {showAccountInfo ? 'Close Settings' : 'Settings'}
          </button>
          <button onClick={logout} style={{ backgroundColor: '#ff4d4d', color: 'white' }}>Logout</button>
        </div>
      </div>

      {showAccountInfo && (
        <div style={{ border: '1px solid #ccc', padding: '20px', margin: '20px 0', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
          <h3>Account Information</h3>
          <p><strong>Email:</strong> {userEmail}</p>
          <p><strong>Status:</strong> Active</p>
          <p><strong>Member Since:</strong> {new Date().getFullYear()}</p>
          <hr />
          <p><em>Order history and profile editing coming soon...</em></p>
        </div>
      )}

      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '20px' }}>
        {products.map((product) => (
          <div key={product.id} style={{ border: '1px solid #ddd', padding: '15px', borderRadius: '8px' }}>
            <h3>{product.name}</h3>
            <p>${product.price}</p>
            <button onClick={() => addToCart(product)}>Add to Cart</button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ProductList;