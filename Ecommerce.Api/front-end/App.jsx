import React, { useState, useEffect } from 'react';
import './App.css';

// --- Mock Data ---
const MOCK_PRODUCTS = [
  {
    id: 1,
    name: "Beosound Balance",
    category: "Audio",
    price: 2250,
    rating: 4.9,
    reviews: 24,
    image: "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&w=800&q=80",
    inStock: true,
  },
  {
    id: 2,
    name: "Chronos Heritage 42mm",
    category: "Timepieces",
    price: 3450,
    rating: 4.8,
    reviews: 18,
    image: "https://images.unsplash.com/photo-1524592094714-0f0654e20314?auto=format&fit=crop&w=800&q=80",
    inStock: true,
  },
  {
    id: 3,
    name: "Cashmere Travel Wrap",
    category: "Apparel",
    price: 495,
    rating: 5.0,
    reviews: 42,
    image: "https://images.unsplash.com/photo-1520903920243-00d872a2d1c9?auto=format&fit=crop&w=800&q=80",
    inStock: true,
  },
  {
    id: 4,
    name: "Leica M11 Rangefinder",
    category: "Photography",
    price: 8995,
    rating: 4.9,
    reviews: 7,
    image: "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?auto=format&fit=crop&w=800&q=80",
    inStock: false,
  }
];

// --- Components ---

const Navbar = ({ user, onLogin, onLogout, cartCount, onOpenCart }) => (
  <header className="nav">
    <div className="nav-left">
      <a className="brand" href="#">
        <span className="brand-badge">L</span>
        <span className="brand-name">Langster</span>
      </a>
    </div>

    <nav className="nav-center" aria-label="Primary">
      <a href="#" aria-current="page" className="nav-link active">Home</a>
      <a href="#" className="nav-link">Products</a>
    </nav>

    <div className="nav-right">
      <button className="icon-btn" aria-label="Search">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      </button>
      <button className="icon-btn cart-btn" aria-label="Cart" onClick={onOpenCart}>
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
        {cartCount > 0 && (
          <span className="cart-count">{cartCount}</span>
        )}
      </button>
      {user ? (
        <button className="user-chip" onClick={onLogout} title="Click to Sign Out">
          <span className="user-dot" aria-hidden="true"></span>
          <span className="user-name">{user.name}</span>
        </button>
      ) : (
        <button className="btn-login" onClick={onLogin}>Sign In</button>
      )}
    </div>
  </header>
);

const ProductCard = ({ product, applyDiscount, onAddToCart }) => {
  const [isLiked, setIsLiked] = useState(false);
  
  const finalPrice = applyDiscount ? product.price * 0.8 : product.price;

  return (
    <article className="card">
      <div className="card-image-wrapper">
        <img 
          src={product.image} 
          alt={product.name} 
          className="card-image" 
          loading="lazy"
        />
        <div className="card-badge">Featured</div>
        <button 
          className={`card-heart ${isLiked ? 'active' : ''}`}
          onClick={() => setIsLiked(!isLiked)}
          aria-label={isLiked ? "Remove from wishlist" : "Add to wishlist"}
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill={isLiked ? "currentColor" : "none"} stroke="currentColor" strokeWidth="2">
            <path d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z" strokeLinecap="round" strokeLinejoin="round"/>
          </svg>
        </button>
        <div className="card-overlay">
          <button 
            className="btn-view"
            onClick={() => onAddToCart(product)}
          >
            Add to Cart
          </button>
        </div>
      </div>
      
      <div className="card-content">
        <div className="card-meta">
          <div className="stars" aria-label={`${product.rating} out of 5 stars`}>
            {'★'.repeat(Math.floor(product.rating))}
            <span className="stars-muted">{'★'.repeat(5 - Math.floor(product.rating))}</span>
            <span className="review-count">({product.reviews})</span>
          </div>
          <span className="category-label">{product.category}</span>
        </div>
        
        <h3 className="product-title" title={product.name}>{product.name}</h3>
        
        <div className="card-footer">
          <div className="price-wrapper">
            {applyDiscount && (
              <span className="price-original">${product.price.toLocaleString()}</span>
            )}
            <span className={`price ${applyDiscount ? 'price-discounted' : ''}`}>
              ${finalPrice.toLocaleString(undefined, { maximumFractionDigits: 0 })}
            </span>
          </div>
          {product.inStock ? (
            <span className="stock-status in-stock">In Stock</span>
          ) : (
            <span className="stock-status out-stock">Sold Out</span>
          )}
        </div>
      </div>
    </article>
  );
};

const SkeletonCard = () => (
  <div className="card skeleton">
    <div className="skeleton-image"></div>
    <div className="skeleton-line w-60"></div>
    <div className="skeleton-line w-80"></div>
    <div className="skeleton-line w-40"></div>
  </div>
);

const CartDrawer = ({ isOpen, onClose, cart, user, onRemove, onCheckout }) => {
  const [checkoutState, setCheckoutState] = useState('idle'); // idle, processing, success

  // Reset state when drawer opens/closes
  useEffect(() => {
    if (isOpen && checkoutState === 'success') {
      setCheckoutState('idle');
    }
  }, [isOpen]);

  if (!isOpen) return null;

  const calculateTotal = () => {
    return cart.reduce((total, item) => {
      const price = user ? item.price * 0.8 : item.price;
      return total + price;
    }, 0);
  };

  const handlePayment = () => {
    setCheckoutState('processing');
    setTimeout(() => {
      setCheckoutState('success');
      onCheckout(); // Clear cart in parent
    }, 2000);
  };

  return (
    <div className="cart-overlay">
      <div className="cart-drawer">
        <div className="cart-header">
          <h2>Your Cart ({cart.length})</h2>
          <button onClick={onClose} className="close-btn">×</button>
        </div>

        {checkoutState === 'success' ? (
          <div className="cart-success">
            <div className="success-icon">🎉</div>
            <h3>Order Processed!</h3>
            <p>Thank you for your purchase.</p>
            <button onClick={onClose} className="btn-primary full-width">Continue Shopping</button>
          </div>
        ) : (
          <>
            <div className="cart-items">
              {cart.length === 0 ? (
                <p className="empty-cart">Your cart is empty.</p>
              ) : (
                cart.map((item, index) => (
                  <div key={`${item.id}-${index}`} className="cart-item">
                    <img src={item.image} alt={item.name} />
                    <div className="cart-item-info">
                      <h4>{item.name}</h4>
                      <p>${(user ? item.price * 0.8 : item.price).toLocaleString()}</p>
                    </div>
                    <button onClick={() => onRemove(index)} className="remove-btn">Remove</button>
                  </div>
                ))
              )}
            </div>

            {cart.length > 0 && (
              <div className="cart-footer">
                <div className="cart-total">
                  <span>Total</span>
                  <span>${calculateTotal().toLocaleString()}</span>
                </div>
                <button 
                  className="btn-primary full-width" 
                  onClick={handlePayment}
                  disabled={checkoutState === 'processing'}
                >
                  {checkoutState === 'processing' ? 'Processing...' : 'Pay Now'}
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default function App() {
  const [user, setUser] = useState(null); // Default to Guest (null)
  const [products, setProducts] = useState([]);
  const [cart, setCart] = useState([]);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [loading, setLoading] = useState(true);

  const handleLogin = () => setUser({ name: 'Shelton Bumhe' });
  const handleLogout = () => setUser(null);
  
  const addToCart = (product) => {
    setCart([...cart, product]);
    setIsCartOpen(true);
  };

  const removeFromCart = (indexToRemove) => {
    setCart(cart.filter((_, index) => index !== indexToRemove));
  };

  const clearCart = () => {
    setCart([]);
  };

  useEffect(() => {
    // Simulate API fetch delay
    const timer = setTimeout(() => {
      setProducts(MOCK_PRODUCTS);
      setLoading(false);
    }, 1200);
    return () => clearTimeout(timer);
  }, []);

  return (
    <div className="page">
      <Navbar 
        user={user} 
        onLogin={handleLogin} 
        onLogout={handleLogout} 
        cartCount={cart.length}
        onOpenCart={() => setIsCartOpen(true)}
      />
      
      <main className="main-container">
        {!user && (
          <div className="promo-banner">
            <span>🎉 <strong>Guest Offer:</strong> Log in now to save <strong>20%</strong> on all products!</span>
            <button className="btn-link" onClick={handleLogin}>Login to Save</button>
          </div>
        )}

        <section className="featured-section" aria-labelledby="featured-title">
          <header className="section-header">
            <div className="header-left">
              <span className="kicker">Curated Selection</span>
              <h2 id="featured-title" className="section-title">Featured Products</h2>
              <p className="section-subtitle">Hand-picked essentials for the modern connoisseur.</p>
            </div>
            <a href="#" className="view-all-link">
              View All Products <span aria-hidden="true">→</span>
            </a>
          </header>

          <div className="product-grid">
            {loading ? (
              <>
                <SkeletonCard />
                <SkeletonCard />
                <SkeletonCard />
                <SkeletonCard />
              </>
            ) : products.length > 0 ? (
              products.map(product => (
                <ProductCard 
                  key={product.id} 
                  product={product} 
                  applyDiscount={!!user} 
                  onAddToCart={addToCart}
                />
              ))
            ) : (
              <div className="empty-state">No products found.</div>
            )}
          </div>
        </section>
      </main>

      <CartDrawer 
        isOpen={isCartOpen} 
        onClose={() => setIsCartOpen(false)} 
        cart={cart}
        user={user}
        onRemove={removeFromCart}
        onCheckout={clearCart}
      />
    </div>
  );
}