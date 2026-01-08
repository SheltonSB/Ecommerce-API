import React, { useState, useEffect } from 'react';
import { API_URL } from './config';

const AdminDashboard = () => {
  const [products, setProducts] = useState([]);
  const [view, setView] = useState('list'); // 'list' or 'form'
  const [editingProduct, setEditingProduct] = useState(null);
  const [categories, setCategories] = useState([]); // Ideally fetch these from API

  // Auth Header Helper
  const getHeaders = () => {
    const token = localStorage.getItem('token');
    return {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    };
  };

  useEffect(() => {
    fetchProducts();
    // Mock categories for now, or fetch from /api/categories
    setCategories([{ id: 1, name: 'Electronics' }, { id: 2, name: 'Home' }, { id: 3, name: 'Clothing' }]);
  }, []);

  const fetchProducts = async () => {
    try {
      const res = await fetch(`${API_URL}/products`, { headers: getHeaders() });
      if (res.ok) {
        const data = await res.json();
        setProducts(data.items || data); // Handle paged or list response
      }
    } catch (err) {
      console.error("Failed to fetch products", err);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this product?")) return;
    try {
      await fetch(`${API_URL}/products/${id}`, { method: 'DELETE', headers: getHeaders() });
      fetchProducts();
    } catch (err) {
      alert("Failed to delete");
    }
  };

  const handleSave = async (formData) => {
    const method = editingProduct ? 'PUT' : 'POST';
    const url = editingProduct ? `${API_URL}/products/${editingProduct.id}` : `${API_URL}/products`;
    
    try {
      const res = await fetch(url, {
        method: method,
        headers: getHeaders(),
        body: JSON.stringify(formData)
      });
      
      if (!res.ok) {
        const err = await res.json();
        alert(`Error: ${JSON.stringify(err)}`);
        return;
      }
      
      setView('list');
      setEditingProduct(null);
      fetchProducts();
    } catch (err) {
      console.error("Save failed", err);
    }
  };

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', borderBottom: '1px solid #eee', paddingBottom: '10px' }}>
        <h1>🛒 Enterprise Admin Portal</h1>
        <div>
          <button onClick={() => { setEditingProduct(null); setView('form'); }} style={styles.btnPrimary}>+ Add New Product</button>
          <button onClick={() => window.location.href='/'} style={styles.btnSecondary}>Back to Store</button>
        </div>
      </header>

      {view === 'list' ? (
        <ProductTable products={products} onEdit={(p) => { setEditingProduct(p); setView('form'); }} onDelete={handleDelete} />
      ) : (
        <ProductForm product={editingProduct} onSave={handleSave} onCancel={() => setView('list')} categories={categories} />
      )}
    </div>
  );
};

// --- Sub-Components ---

const ProductTable = ({ products, onEdit, onDelete }) => (
  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
    <thead>
      <tr style={{ background: '#f4f4f4', textAlign: 'left' }}>
        <th style={styles.th}>Status</th>
        <th style={styles.th}>Quality</th>
        <th style={styles.th}>SKU</th>
        <th style={styles.th}>Name</th>
        <th style={styles.th}>Price</th>
        <th style={styles.th}>Stock</th>
        <th style={styles.th}>Actions</th>
      </tr>
    </thead>
    <tbody>
      {products.map(p => (
        <tr key={p.id} style={{ borderBottom: '1px solid #eee' }}>
          <td style={styles.td}><StatusBadge status={p.isActive ? 'Active' : 'Draft'} /></td>
          <td style={styles.td}>{p.listingQualityScore || 0}%</td>
          <td style={styles.td}>{p.sku}</td>
          <td style={styles.td}>{p.name}</td>
          <td style={styles.td}>${p.price}</td>
          <td style={styles.td}>{p.stockQuantity}</td>
          <td style={styles.td}>
            <button onClick={() => onEdit(p)} style={styles.btnSmall}>Edit</button>
            <button onClick={() => onDelete(p.id)} style={{...styles.btnSmall, background: '#ff4d4d'}}>Delete</button>
          </td>
        </tr>
      ))}
    </tbody>
  </table>
);

const ProductForm = ({ product, onSave, onCancel, categories }) => {
  const [tab, setTab] = useState('basic');
  const [formData, setFormData] = useState({
    name: '', description: '', price: 0, sku: '', stockQuantity: 0, categoryId: 1, isActive: false,
    upc: '', gtin: '', keyFeatures: '', inventoryLocation: '',
    weight: 0, height: 0, width: 0, length: 0,
    isHazmat: false, safetyDataSheetUrl: '', floorPrice: 0, ceilingPrice: 0,
    imageUrl: '',
    ...product // Override defaults if editing
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <div style={{ background: '#fff', padding: '20px', borderRadius: '8px', boxShadow: '0 2px 10px rgba(0,0,0,0.1)' }}>
      <div style={{ marginBottom: '20px', borderBottom: '1px solid #ddd' }}>
        {['basic', 'logistics', 'compliance', 'pricing'].map(t => (
          <button 
            key={t} 
            onClick={() => setTab(t)}
            style={{ 
              padding: '10px 20px', 
              background: 'none', 
              border: 'none', 
              borderBottom: tab === t ? '3px solid #007bff' : '3px solid transparent',
              fontWeight: 'bold',
              cursor: 'pointer',
              textTransform: 'capitalize'
            }}
          >
            {t} Info
          </button>
        ))}
      </div>

      <form onSubmit={handleSubmit}>
        {tab === 'basic' && (
          <div style={styles.grid}>
            <label style={styles.label}>Product Name <input name="name" value={formData.name} onChange={handleChange} style={styles.input} required /></label>
            <label style={styles.label}>SKU <input name="sku" value={formData.sku} onChange={handleChange} style={styles.input} required /></label>
            <label style={styles.label}>Category 
              <select name="categoryId" value={formData.categoryId} onChange={handleChange} style={styles.input}>
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </label>
            <label style={styles.label}>Description <textarea name="description" value={formData.description} onChange={handleChange} style={styles.textarea} /></label>
            <label style={styles.label}>Key Features (Bullet Points) <textarea name="keyFeatures" value={formData.keyFeatures} onChange={handleChange} style={styles.textarea} placeholder="• Feature 1&#10;• Feature 2" /></label>
            <label style={styles.label}>Image URL <input name="imageUrl" value={formData.imageUrl} onChange={handleChange} style={styles.input} /></label>
          </div>
        )}

        {tab === 'logistics' && (
          <div style={styles.grid}>
            <h3>Dimensions & Weight (Cubiscan)</h3>
            <div style={{ display: 'flex', gap: '10px' }}>
              <label style={styles.label}>Weight (lbs) <input type="number" name="weight" value={formData.weight} onChange={handleChange} style={styles.input} /></label>
              <label style={styles.label}>Length (in) <input type="number" name="length" value={formData.length} onChange={handleChange} style={styles.input} /></label>
              <label style={styles.label}>Width (in) <input type="number" name="width" value={formData.width} onChange={handleChange} style={styles.input} /></label>
              <label style={styles.label}>Height (in) <input type="number" name="height" value={formData.height} onChange={handleChange} style={styles.input} /></label>
            </div>
            <h3>Identification</h3>
            <label style={styles.label}>UPC / Barcode <input name="upc" value={formData.upc} onChange={handleChange} style={styles.input} /></label>
            <label style={styles.label}>GTIN <input name="gtin" value={formData.gtin} onChange={handleChange} style={styles.input} /></label>
            <label style={styles.label}>Inventory Location <input name="inventoryLocation" value={formData.inventoryLocation} onChange={handleChange} style={styles.input} placeholder="e.g. Warehouse A, Bin 42" /></label>
            <label style={styles.label}>Stock Quantity <input type="number" name="stockQuantity" value={formData.stockQuantity} onChange={handleChange} style={styles.input} /></label>
          </div>
        )}

        {tab === 'compliance' && (
          <div style={styles.grid}>
            <h3>Safety & Compliance</h3>
            <label style={{ display: 'flex', alignItems: 'center', gap: '10px', margin: '10px 0' }}>
              <input type="checkbox" name="isHazmat" checked={formData.isHazmat} onChange={handleChange} />
              Is Hazardous Material? (Battery, Chemical, etc.)
            </label>
            {formData.isHazmat && (
              <label style={styles.label}>Safety Data Sheet (SDS) URL <input name="safetyDataSheetUrl" value={formData.safetyDataSheetUrl} onChange={handleChange} style={styles.input} required /></label>
            )}
            <div style={{ background: '#fff3cd', padding: '10px', borderRadius: '5px', fontSize: '0.9em' }}>
              ⚠️ <strong>Compliance Bot:</strong> Products with restricted keywords (e.g., "Cures Cancer") will be automatically blocked upon save.
            </div>
          </div>
        )}

        {tab === 'pricing' && (
          <div style={styles.grid}>
            <h3>Pricing Strategy</h3>
            <label style={styles.label}>Selling Price ($) <input type="number" name="price" value={formData.price} onChange={handleChange} style={styles.input} required /></label>
            <div style={{ display: 'flex', gap: '10px', marginTop: '10px' }}>
              <label style={styles.label}>Floor Price (Min) <input type="number" name="floorPrice" value={formData.floorPrice} onChange={handleChange} style={styles.input} /></label>
              <label style={styles.label}>Ceiling Price (Max) <input type="number" name="ceilingPrice" value={formData.ceilingPrice} onChange={handleChange} style={styles.input} /></label>
            </div>
            <p style={{ fontSize: '0.8em', color: '#666' }}>Automated repricing will keep the price between Floor and Ceiling based on competitor data.</p>
          </div>
        )}

        <div style={{ marginTop: '20px', borderTop: '1px solid #eee', paddingTop: '20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <label style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <input type="checkbox" name="isActive" checked={formData.isActive} onChange={handleChange} />
            <strong>Publish to Store (Active)</strong>
          </label>
          <div>
            <button type="button" onClick={onCancel} style={styles.btnSecondary}>Cancel</button>
            <button type="submit" style={styles.btnPrimary}>Save Product</button>
          </div>
        </div>
      </form>
    </div>
  );
};

const StatusBadge = ({ status }) => {
  const colors = {
    'Active': '#28a745',
    'Draft': '#6c757d',
    'Blocked': '#dc3545',
    'PendingReview': '#ffc107'
  };
  return (
    <span style={{ 
      background: colors[status] || '#ccc', 
      color: '#fff', 
      padding: '4px 8px', 
      borderRadius: '12px', 
      fontSize: '0.8em' 
    }}>
      {status}
    </span>
  );
};

const styles = {
  btnPrimary: { background: '#007bff', color: '#fff', border: 'none', padding: '10px 20px', borderRadius: '4px', cursor: 'pointer', marginLeft: '10px' },
  btnSecondary: { background: '#6c757d', color: '#fff', border: 'none', padding: '10px 20px', borderRadius: '4px', cursor: 'pointer' },
  btnSmall: { padding: '5px 10px', margin: '0 2px', borderRadius: '4px', border: 'none', cursor: 'pointer', background: '#007bff', color: '#fff' },
  th: { padding: '12px', borderBottom: '2px solid #ddd' },
  td: { padding: '12px' },
  grid: { display: 'flex', flexDirection: 'column', gap: '15px' },
  label: { display: 'flex', flexDirection: 'column', gap: '5px', fontWeight: 'bold', fontSize: '0.9em' },
  input: { padding: '8px', borderRadius: '4px', border: '1px solid #ccc' },
  textarea: { padding: '8px', borderRadius: '4px', border: '1px solid #ccc', minHeight: '80px' }
};

export default AdminDashboard;