import { useState } from 'react';
import { Button } from '../../components/ui/Button';
import { Card } from '../../components/ui/Card';
import api from '../../api/axios';
import toast from 'react-hot-toast';
import { UploadCloud } from 'lucide-react';

export function AddProduct() {
  const [file, setFile] = useState<File | null>(null);
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [description, setDescription] = useState('');
  const [sku, setSku] = useState('');
  const [stockQuantity, setStockQuantity] = useState('0');
  const [categoryId, setCategoryId] = useState('1');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    const formData = new FormData();
    formData.append('Name', name);
    formData.append('Price', price || '0');
    formData.append('Description', description);
    formData.append('Sku', sku);
    formData.append('StockQuantity', stockQuantity || '0');
    formData.append('CategoryId', categoryId || '1');
    if (file) {
      formData.append('Image', file);
    }

    try {
      await api.post('/admin/products', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      toast.success('Product uploaded');
      setFile(null);
      setName('');
      setPrice('');
      setDescription('');
      setSku('');
      setStockQuantity('0');
    } catch {
      toast.error('Upload failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm">
        <h1 className="text-2xl font-semibold text-gray-900">Admin • Add Product</h1>
        <p className="text-sm text-gray-500">Upload a new product with imagery hosted on Cloudinary.</p>
      </div>
      <Card className="max-w-3xl">
        <form className="space-y-6" onSubmit={handleSubmit}>
          <div>
            <label className="text-sm font-medium text-gray-800">Product image</label>
            <div className="mt-2 rounded-xl border-2 border-dashed border-gray-200 bg-gray-50 px-6 py-8 text-center hover:border-indigo-500 transition">
              <input
                type="file"
                accept="image/*"
                className="absolute inset-0 h-full w-full cursor-pointer opacity-0"
                onChange={(e) => setFile(e.target.files?.[0] || null)}
              />
              <div className="flex flex-col items-center gap-2 text-gray-600">
                <UploadCloud className="h-8 w-8 text-indigo-500" />
                <p className="text-sm">{file ? file.name : 'Drag and drop or click to upload image'}</p>
              </div>
            </div>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">Name</label>
              <input
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">SKU</label>
              <input
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={sku}
                onChange={(e) => setSku(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">Price</label>
              <input
                type="number"
                step="0.01"
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">Stock quantity</label>
              <input
                type="number"
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={stockQuantity}
                onChange={(e) => setStockQuantity(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">Category ID</label>
              <input
                type="number"
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={categoryId}
                onChange={(e) => setCategoryId(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-800">Description</label>
              <input
                className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </div>
          </div>

          <Button type="submit" disabled={loading} className="w-full md:w-auto">
            {loading ? 'Uploading...' : 'Create product'}
          </Button>
        </form>
      </Card>
    </div>
  );
}
