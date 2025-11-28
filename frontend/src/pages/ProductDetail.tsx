import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import api from '../api/axios';
import { mockProducts } from '../data/mockProducts';
import { ShoppingCart } from 'lucide-react';
import toast from 'react-hot-toast';

type Product = {
  id: number;
  name: string;
  description?: string;
  price: number;
  sku: string;
  imageUrl?: string;
};

const placeholderImage = 'https://placehold.co/600x400';

export function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get(`/products/${id}`);
        setProduct(res.data);
      } catch {
        const fallback = mockProducts.find((p) => String(p.id) === id);
        if (fallback) {
          setProduct(fallback);
        } else {
          toast.error('Product not found');
          navigate('/products');
        }
      }
    };
    load();
  }, [id, navigate]);

  if (!product) {
    return <p className="text-sm text-gray-500">Loading product...</p>;
  }

  return (
    <div className="grid gap-10 lg:grid-cols-2">
      <div className="rounded-3xl border border-gray-100 bg-white p-6 shadow-sm">
        <img src={product.imageUrl || placeholderImage} alt={product.name} className="w-full rounded-2xl object-cover" />
      </div>
      <div className="space-y-4">
        <p className="text-sm font-medium text-indigo-700">SKU {product.sku}</p>
        <h1 className="text-3xl font-semibold text-gray-900">{product.name}</h1>
        <p className="text-lg text-gray-600">{product.description}</p>
        <p className="text-3xl font-semibold text-gray-900">${product.price.toFixed(2)}</p>
        <div className="flex gap-3">
          <Button className="flex items-center gap-2" onClick={() => toast.success('Added to cart')}>
            <ShoppingCart className="h-4 w-4" />
            Add to cart
          </Button>
          <Button variant="secondary" onClick={() => navigate('/products')}>
            Back to catalog
          </Button>
        </div>
      </div>
    </div>
  );
}
