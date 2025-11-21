import { useEffect, useState } from 'react';
import { Card } from '../components/ui/Card';
import api from '../api/axios';
import { Button } from '../components/ui/Button';
import { ShoppingCart } from 'lucide-react';
import toast from 'react-hot-toast';

type Product = {
  id: number;
  name: string;
  description?: string;
  price: number;
  sku: string;
};

type PagedResponse<T> = {
  items: T[];
  totalCount?: number;
};

export function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const res = await api.get<PagedResponse<Product>>('/products');
        setProducts(res.data.items ?? res.data);
      } catch (err) {
        // handled by interceptor
      } finally {
        setLoading(false);
      }
    };
    fetchProducts();
  }, []);

  const handleAddToCart = (product: Product) => {
    // placeholder for cart integration
    toast.success(`${product.name} added to cart`);
  };

  if (loading) {
    return <p className="text-sm text-gray-500">Loading products...</p>;
  }

  return (
    <div className="space-y-8">
      <div className="rounded-2xl border border-indigo-100 bg-white/70 p-6 shadow-sm">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-sm font-medium text-indigo-700">Curated for you</p>
            <h1 className="text-2xl font-semibold text-gray-900">LuxeStore Catalog</h1>
            <p className="text-sm text-gray-500">Browse our latest products with a clean, minimal aesthetic.</p>
          </div>
          <Button className="w-full md:w-auto">View Collections</Button>
        </div>
      </div>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {products.map((product) => (
          <Card key={product.id} className="flex flex-col gap-3">
            <div>
              <p className="text-base font-semibold text-gray-900">{product.name}</p>
              <p className="text-sm text-gray-500 line-clamp-2">{product.description}</p>
            </div>
            <div className="mt-auto flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">SKU {product.sku}</p>
                <p className="text-lg font-semibold text-gray-900">${product.price.toFixed(2)}</p>
              </div>
              <Button className="flex items-center gap-2" onClick={() => handleAddToCart(product)}>
                <ShoppingCart className="h-4 w-4" />
                Add
              </Button>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}
