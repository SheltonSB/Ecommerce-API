import React, { useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route, Link, useParams, useNavigate, Navigate, useLocation } from 'react-router-dom';
import { Toaster, toast } from 'react-hot-toast';
import { AuthProvider, useAuth } from './AuthContext';
import { CartProvider, useCart } from './CartContext';
import { track, pageView } from './analytics';
import { API_URL } from './config';
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

const PageTracker = () => {
  const location = useLocation();
  useEffect(() => {
    pageView(location.pathname);
  }, [location]);
  return null;
};

const RequireAuth = ({ children }) => {
  const { user } = useAuth();
  const location = useLocation();

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
};

const Navbar = ({ products = [] }) => {
  const { user, logout } = useAuth();
  const { cart } = useCart();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [suggestions, setSuggestions] = useState([]);
  const navigate = useNavigate();

  // Debounce search requests
  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      if (searchQuery.trim().length > 0) {
        // Track search for ML training
        if (searchQuery.length > 2) track('search_query', { query: searchQuery });

        try {
          // Call backend ML search endpoint instead of local filtering
          const res = await fetch(`${API_URL}/products/search?q=${encodeURIComponent(searchQuery)}`);
          if (res.ok) {
            const data = await res.json();
            setSuggestions(data); // Backend returns ML-based matches or recommendations
          } else {
            throw new Error("Search failed");
          }
        } catch (error) {
          // Fallback to local filtering if backend is offline
          const filtered = products.filter(p => 
            p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            p.category.toLowerCase().includes(searchQuery.toLowerCase())
          ).slice(0, 5);
          
          if (filtered.length === 0) {
            // Fallback logic
            setSuggestions(products.sort((a, b) => b.rating - a.rating).slice(0, 3).map(p => ({...p, isRecommendation: true})));
          } else {
            setSuggestions(filtered);
          }
        }
      } else {
        setSuggestions([]);
      }
    }, 300); // 300ms debounce

    return () => clearTimeout(delayDebounceFn);
  }, [searchQuery, products]);

  const handleSearch = (e) => {
    setSearchQuery(e.target.value);
  };

  const handleSuggestionClick = (productId) => {
    navigate(`/products/${productId}`);
    setSearchQuery('');
    setSuggestions([]);
    track('search_click', { productId });
  };

  return (
    <header className="nav">
    <div className="nav-left">
      <Link className="brand" to="/">
        <span className="brand-badge">L</span>
        <span className="brand-name">Langster</span>
      </Link>
    </div>

    <nav className="nav-center" aria-label="Primary">
      <Link to="/" className="nav-link">Home</Link>
      <Link to="/" className="nav-link">Products</Link>
    </nav>

    <div className="nav-right">
      <button className="icon-btn hamburger" aria-label="Open menu" onClick={() => setIsMenuOpen(!isMenuOpen)}>
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M4 6h16M4 12h16M4 18h16" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      </button>
      
      <div className="search-container">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
        <input 
          type="text" 
          className="search-input" 
          placeholder="Search..." 
          value={searchQuery}
          onChange={handleSearch}
        />
        {suggestions.length > 0 && (
          <div className="search-suggestions">
            {suggestions[0].isRecommendation && (
              <div className="suggestion-header">No matches. Recommended for you:</div>
            )}
            {suggestions.map(product => (
              <div key={product.id} className="suggestion-item" onClick={() => handleSuggestionClick(product.id)}>
                <img src={product.image} alt="" className="suggestion-thumb" />
                <div className="suggestion-info">
                  <span className="suggestion-name">{product.name}</span>
                  <span className="suggestion-price">${product.price.toLocaleString()}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <Link to="/cart" className="icon-btn cart-btn" aria-label="Cart">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
        {cart.length > 0 && (
          <span className="cart-count">{cart.length}</span>
        )}
      </Link>
      {user ? (
        <button className="user-chip" onClick={logout} title="Click to Sign Out">
          <span className="user-dot" aria-hidden="true"></span>
          <span className="user-name">{user.name}</span>
        </button>
      ) : (
        <Link className="btn-login" to="/login">Sign In</Link>
      )}
    </div>

    {isMenuOpen && (
      <div className="mobile-menu">
        <Link to="/" className="nav-link" onClick={() => setIsMenuOpen(false)}>Home</Link>
        <Link to="/" className="nav-link" onClick={() => setIsMenuOpen(false)}>Products</Link>
        {!user && <Link to="/login" className="nav-link" onClick={() => setIsMenuOpen(false)}>Sign In</Link>}
      </div>
    )}
    </header>
  );
};

const ProductCard = ({ product }) => {
  const { user } = useAuth();
  const { addToCart } = useCart();
  const [isLiked, setIsLiked] = useState(false);
  const [imgSrc, setImgSrc] = useState(product.image);
  
  const finalPrice = user ? product.price * 0.8 : product.price;

  const handleAddToCart = () => {
    addToCart(product);
    toast.success(`Added ${product.name} to cart!`);
    track('add_to_cart', { productId: product.id, name: product.name, price: finalPrice });
  };

  return (
    <article className="card">
      <div className="card-image-wrapper">
        <Link to={`/products/${product.id}`}>
          <img 
            src={imgSrc} 
            alt={product.name} 
            className="card-image" 
            loading="lazy"
            onError={() => setImgSrc('https://placehold.co/400x500?text=No+Image')}
          />
        </Link>
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
            onClick={handleAddToCart}
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
        
        <Link to={`/products/${product.id}`} className="product-title-link">
          <h3 className="product-title" title={product.name}>{product.name}</h3>
        </Link>
        
        <div className="card-footer">
          <div className="price-wrapper">
            {user && (
              <span className="price-original">${product.price.toLocaleString()}</span>
            )}
            <span className={`price ${user ? 'price-discounted' : ''}`}>
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

// --- Pages ---

const HomePage = ({ products, loading }) => {
  const { user } = useAuth();

  return (
    <div className="main-container">
    {!user && (
      <div className="promo-banner">
        <span>🎉 <strong>Guest Offer:</strong> Log in now to save <strong>20%</strong> on all products!</span>
        <Link className="btn-link" to="/login">Login to Save</Link>
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
            />
          ))
        ) : (
          <div className="empty-state">No products found.</div>
        )}
      </div>
    </section>
    </div>
  );
};

const ProductDetailsPage = () => {
  const { user } = useAuth();
  const { addToCart } = useCart();
  const { id } = useParams();
  const product = MOCK_PRODUCTS.find(p => p.id === parseInt(id));
  
  useEffect(() => {
    if (product) {
      const startTime = Date.now();
      return () => {
        const duration = (Date.now() - startTime) / 1000;
        track('product_view_duration', {
          productId: product.id,
          name: product.name,
          duration_seconds: duration
        });
      };
    }
  }, [product]);

  if (!product) return <div className="main-container"><p>Product not found</p></div>;

  const finalPrice = user ? product.price * 0.8 : product.price;

  return (
    <div className="main-container product-details-page">
      <div className="details-image">
        <img src={product.image} alt={product.name} />
      </div>
      <div className="details-info">
        <span className="category-label">{product.category}</span>
        <h1>{product.name}</h1>
        <div className="stars">{'★'.repeat(Math.floor(product.rating))} <span className="review-count">({product.reviews} reviews)</span></div>
        
        <div className="price-wrapper large-price">
          {user && <span className="price-original">${product.price.toLocaleString()}</span>}
          <span className={`price ${user ? 'price-discounted' : ''}`}>
            ${finalPrice.toLocaleString(undefined, { maximumFractionDigits: 0 })}
          </span>
        </div>

        <p className="description">Experience the pinnacle of design and functionality. This premium {product.category.toLowerCase()} item is crafted for those who demand excellence.</p>
        
        <div className="options">
          <div className="option-group">
            <label>Color</label>
            <div className="color-swatches">
              <button className="swatch selected" style={{background: '#111'}}></button>
              <button className="swatch" style={{background: '#888'}}></button>
              <button className="swatch" style={{background: '#d6a94f'}}></button>
            </div>
          </div>
        </div>

        <button onClick={() => addToCart(product)} className="btn-primary full-width">Add to Cart</button>
      </div>
    </div>
  );
};

const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      await login(email, password);
      toast.success("Welcome back!");
      navigate(from, { replace: true });
    } catch (error) {
      toast.error("Invalid email or password");
    } finally {
      setIsLoading(false);
    }
  };
  return (
    <div className="main-container auth-page">
      <div className="auth-card">
        <h2>Welcome Back</h2>
        <form onSubmit={handleSubmit}>
          <input 
            type="email" 
            placeholder="Email Address" 
            required 
            className="input-field"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input 
            type="password" 
            placeholder="Password" 
            required 
            className="input-field"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <button type="submit" className="btn-primary full-width" disabled={isLoading}>
            {isLoading ? 'Signing In...' : 'Sign In'}
          </button>
        </form>
        <p className="auth-footer">Don't have an account? <Link to="/register">Register</Link></p>
      </div>
    </div>
  );
};

const RegisterPage = () => (
  <div className="main-container auth-page">
    <div className="auth-card">
      <h2>Create Account</h2>
      <form onSubmit={(e) => e.preventDefault()}>
        <input type="text" placeholder="Full Name" required className="input-field" />
        <input type="email" placeholder="Email Address" required className="input-field" />
        <input type="password" placeholder="Password" required className="input-field" />
        <button type="submit" className="btn-primary full-width">Sign Up</button>
      </form>
      <p className="auth-footer">Already have an account? <Link to="/login">Sign In</Link></p>
    </div>
  </div>
);

const CartPage = () => {
  const { cart, removeFromCart, clearCart } = useCart();
  const { user } = useAuth();
  const [checkoutState, setCheckoutState] = useState('idle'); // idle, processing, success
  const navigate = useNavigate();

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
      clearCart();
      setTimeout(() => navigate('/'), 2000);
    }, 2000);
  };

  return (
    <div className="main-container cart-page">
      <div className="cart-container">
        <h2>Your Cart ({cart.length})</h2>

        {checkoutState === 'success' ? (
          <div className="cart-success">
            <div className="success-icon">🎉</div>
            <h3>Order Processed!</h3>
            <p>Thank you for your purchase.</p>
            <button onClick={() => navigate('/')} className="btn-primary full-width">Continue Shopping</button>
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
                    <button onClick={() => removeFromCart(index)} className="remove-btn">Remove</button>
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
                  {checkoutState === 'processing' ? 'Processing...' : 'Proceed to Checkout'}
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

const CheckoutPage = () => (
  <div className="main-container">
    <h1>Checkout</h1>
    <p>Please use the cart page to proceed with payment.</p>
    <Link to="/cart" className="btn-primary">Go to Cart</Link>
  </div>
);

const NotFoundPage = () => (
  <div className="main-container error-page">
    <h1>404</h1>
    <p>Page not found</p>
    <Link to="/" className="btn-primary">Return Home</Link>
  </div>
);

// --- Main App ---

export default function App() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await fetch(`${API_URL}/products`);
        if (!response.ok) throw new Error('Failed to fetch products');
        const data = await response.json();
        setProducts(data);
      } catch (error) {
        console.error(`Backend connection failed to ${API_URL}:`, error);
        toast.error("Using offline mode (Backend unreachable)");
        setProducts(MOCK_PRODUCTS);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <div className="page">
            <Toaster 
              position="bottom-right"
              toastOptions={{
                style: { background: '#1f2937', color: '#fff' },
                success: { iconTheme: { primary: '#10b981', secondary: '#fff' } }
              }}
            />
            <PageTracker />
            <Navbar products={products} />
            <Routes>
              <Route path="/" element={<HomePage products={products} loading={loading} />} />
              <Route path="/products/:id" element={<ProductDetailsPage products={products} />} />
              <Route path="/cart" element={<CartPage />} />
              <Route path="/checkout" element={
                <RequireAuth>
                  <CheckoutPage />
                </RequireAuth>
              } />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
          </div>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}