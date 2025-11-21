import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { ShoppingBag, Shield, Truck, Sparkles } from 'lucide-react';

const highlights = [
  {
    title: 'Curated Collections',
    description: 'Minimal, elevated products for everyday use.',
    icon: <Sparkles className="h-5 w-5 text-indigo-600" />
  },
  {
    title: 'Fast Delivery',
    description: 'Track shipments with real-time updates.',
    icon: <Truck className="h-5 w-5 text-indigo-600" />
  },
  {
    title: 'Secure Checkout',
    description: 'Protected payments and verified accounts.',
    icon: <Shield className="h-5 w-5 text-indigo-600" />
  }
];

export function Home() {
  return (
    <div className="space-y-10">
      <section className="overflow-hidden rounded-3xl border border-indigo-100 bg-gradient-to-br from-white via-indigo-50 to-white shadow-sm">
        <div className="grid gap-8 p-8 md:grid-cols-2 md:p-12">
          <div className="flex flex-col justify-center space-y-4">
            <div className="inline-flex items-center gap-2 rounded-full bg-indigo-50 px-3 py-1 text-xs font-semibold text-indigo-700">
              <Sparkles className="h-4 w-4" />
              LuxeStore
            </div>
            <h1 className="text-3xl font-semibold text-gray-900 md:text-4xl">Minimal. Modern. Made for you.</h1>
            <p className="text-base text-gray-600">
              Discover a curated selection of products with a refined, clutter-free experience. Shop confidently with verified
              accounts and secure checkout.
            </p>
            <div className="flex flex-col gap-3 sm:flex-row">
              <Button className="w-full sm:w-auto" onClick={() => (window.location.href = '/products')}>
                Browse products
              </Button>
              <Button variant="secondary" className="w-full sm:w-auto" onClick={() => (window.location.href = '/login')}>
                Log in
              </Button>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-500">
              <Shield className="h-4 w-4 text-indigo-600" />
              Email verification required for secure checkout
            </div>
          </div>
          <div className="relative">
            <div className="absolute inset-0 -z-10 bg-gradient-to-tr from-indigo-100 via-white to-indigo-50 blur-3xl" />
            <div className="flex h-full items-center justify-center">
              <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-lg ring-1 ring-gray-100">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-indigo-600 text-white shadow-sm">
                    <ShoppingBag className="h-6 w-6" />
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-gray-900">Featured Drop</p>
                    <p className="text-xs text-gray-500">Handpicked for a clean aesthetic</p>
                  </div>
                </div>
                <div className="mt-6 space-y-4">
                  <div className="flex justify-between text-sm text-gray-700">
                    <span>Premium Desk Lamp</span>
                    <span className="font-semibold text-gray-900">$129.00</span>
                  </div>
                  <div className="flex justify-between text-sm text-gray-700">
                    <span>Wireless Headphones</span>
                    <span className="font-semibold text-gray-900">$199.00</span>
                  </div>
                  <div className="flex justify-between text-sm text-gray-700">
                    <span>Minimal Backpack</span>
                    <span className="font-semibold text-gray-900">$89.00</span>
                  </div>
                  <div className="flex items-center justify-between rounded-xl bg-gray-50 px-4 py-3 text-sm text-gray-700">
                    <span>Total</span>
                    <span className="text-lg font-semibold text-gray-900">$417.00</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-3">
        {highlights.map((item) => (
          <Card key={item.title} className="flex h-full flex-col gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-50">{item.icon}</div>
            <div className="space-y-2">
              <p className="text-base font-semibold text-gray-900">{item.title}</p>
              <p className="text-sm text-gray-600">{item.description}</p>
            </div>
          </Card>
        ))}
      </section>
    </div>
  );
}
