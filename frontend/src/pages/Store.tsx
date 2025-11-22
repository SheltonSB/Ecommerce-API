import { useEffect, useState } from 'react';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { ShoppingCart, Shield, Sparkles, Truck, ArrowRight } from 'lucide-react';
import api from '../api/axios';
import toast from 'react-hot-toast';
import { mockProducts } from '../data/mockProducts';

type Product = {
  id: number;
  name: string;
  description?: string;
  price: number;
  sku: string;
  imageUrl?: string;
  categoryName?: string;
};

type PagedResponse<T> = {
  items: T[];
  totalCount?: number;
};

const storeFeatures = [
  { title: 'Curated minimalism', copy: 'Calm, luxe layouts inspired by high-end editorial design.', icon: <Sparkles className="h-5 w-5 text-indigo-600" /> },
  { title: 'Secure checkout', copy: 'Identity + email verification with JWT auth for trust.', icon: <Shield className="h-5 w-5 text-indigo-600" /> },
  { title: 'Fast fulfillment', copy: 'Stripe-ready plumbing and optimized flows.', icon: <Truck className="h-5 w-5 text-indigo-600" /> }
];

export function Store() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get<PagedResponse<Product>>('/products');
        const incoming = (res.data as any)?.items ?? (res.data as any) ?? [];
        setProducts(Array.isArray(incoming) && incoming.length > 0 ? incoming : mockProducts);
      } catch {
        setError('Failed to load products');
        setProducts(mockProducts);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handleAddToCart = (p: Product) => {
    toast.success(`${p.name} added to cart`);
  };

  return (
    <div className="space-y-12">
      <section className="overflow-hidden rounded-3xl border border-indigo-100 bg-gradient-to-br from-white via-indigo-50 to-white shadow-sm">
        <div className="grid gap-10 p-8 md:grid-cols-2 md:p-12">
          <div className="flex flex-col justify-center space-y-5">
            <div className="inline-flex items-center gap-2 rounded-full bg-indigo-50 px-3 py-1 text-xs font-semibold text-indigo-700">
              <Sparkles className="h-4 w-4" />
              LuxeStore
            </div>
            <h1 className="text-4xl font-semibold text-gray-900 md:text-5xl leading-tight">Timeless Essentials.</h1>
            <p className="text-lg text-gray-600">
              A Havinic-inspired luxury storefront with curated goods, refined typography, and calm shopping flow.
            </p>
            <div className="flex flex-col gap-3 sm:flex-row">
              <Button className="w-full sm:w-auto" onClick={() => (window.location.href = '#catalog')}>
                Browse collection
              </Button>
              <Button variant="secondary" className="w-full sm:w-auto" onClick={() => (window.location.href = '/login')}>
                Admin / Login
              </Button>
            </div>
          </div>
          <div className="relative">
            <div className="absolute inset-0 -z-10 bg-gradient-to-tr from-indigo-100 via-white to-indigo-50 blur-3xl" />
            <div className="grid gap-4 rounded-2xl bg-white p-6 shadow-lg ring-1 ring-gray-100">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-900">Featured set</p>
                  <p className="text-xs text-gray-500">Curated drops from this season</p>
                </div>
                <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-600 text-white">
                  <ShoppingCart className="h-5 w-5" />
                </div>
              </div>
              <div className="space-y-3">
                {mockProducts.slice(0, 3).map((p) => (
                  <div key={p.id} className="flex items-center justify-between rounded-xl bg-gray-50 px-4 py-3 text-sm text-gray-700">
                    <span>{p.name}</span>
                    <span className="font-semibold text-gray-900">${p.price.toFixed(2)}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-3">
        {storeFeatures.map((f) => (
          <Card key={f.title} className="flex h-full flex-col gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-50">{f.icon}</div>
            <div className="space-y-2">
              <p className="text-base font-semibold text-gray-900">{f.title}</p>
              <p className="text-sm text-gray-600">{f.copy}</p>
            </div>
          </Card>
        ))}
      </section>

      <section id="catalog" className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm font-medium text-indigo-700">Catalog</p>
            <h2 className="text-2xl font-semibold text-gray-900">Featured products</h2>
            <p className="text-sm text-gray-500">Live from API when available, otherwise from curated mock data.</p>
          </div>
          <Button variant="secondary" onClick={() => (window.location.href = '/products')}>
            View all
          </Button>
        </div>
        {loading && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 6 }).map((_, idx) => (
              <div key={idx} className="h-48 animate-pulse rounded-xl bg-muted" />
            ))}
          </div>
        )}
        {error && <p className="text-sm text-red-500">{error}</p>}
        {!loading && !error && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {products.map((product) => (
              <Card key={product.id} className="flex flex-col gap-3 overflow-hidden">
                {product.imageUrl ? (
                  <div className="relative h-40 w-full overflow-hidden rounded-lg bg-gray-100">
                    <img src={product.imageUrl} alt={product.name} className="h-full w-full object-cover transition-transform duration-300 hover:scale-105" />
                  </div>
                ) : (
                  <div className="flex h-40 w-full items-center justify-center rounded-lg bg-gray-100 text-sm text-gray-400">No image</div>
                )}
                <div>
                  <p className="text-base font-semibold text-gray-900">{product.name}</p>
                  <p className="text-sm text-gray-500 line-clamp-2">{product.description}</p>
                </div>
                <div className="mt-auto flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-500">SKU {product.sku}</p>
                    <p className="text-lg font-semibold text-gray-900">${product.price.toFixed(2)}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="secondary" onClick={() => (window.location.href = `/products/${product.id}`)}>
                      View
                    </Button>
                    <Button className="flex items-center gap-1" onClick={() => handleAddToCart(product)}>
                      <ShoppingCart className="h-4 w-4" /> Add
                    </Button>
                  </div>
                </div>
              </Card>
            ))}
          </div>
        )}
      </section>

      <section className="rounded-2xl border border-indigo-100 bg-white p-6 shadow-sm">
        <div className="grid gap-4 md:grid-cols-3">
          <div className="md:col-span-2 space-y-2">
            <p className="text-sm font-medium text-indigo-700">Collections</p>
            <h3 className="text-2xl font-semibold text-gray-900">Join the next release</h3>
            <p className="text-sm text-gray-600">Stay ahead with curated drops and early access for verified members.</p>
          </div>
          <div className="flex items-center justify-end">
            <Button className="flex items-center gap-2">
              Get early access <ArrowRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
}
