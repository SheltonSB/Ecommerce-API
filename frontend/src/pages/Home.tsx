import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { ShoppingBag, Shield, Truck, Sparkles, Sparkle, Star } from 'lucide-react';
import { mockProducts } from '../data/mockProducts';
import { cn } from '@/lib/utils';

const features = [
  { title: 'Curated minimalism', copy: 'Havinic-inspired layouts with calm whitespace and premium typography.', icon: <Sparkles className="h-5 w-5 text-indigo-600" /> },
  { title: 'Secure loyalty', copy: 'Identity + email verification and JWT auth for trusted checkout.', icon: <Shield className="h-5 w-5 text-indigo-600" /> },
  { title: 'Fast fulfillment', copy: 'Stripe-ready plumbing and clean admin tools for uploads and inventory.', icon: <Truck className="h-5 w-5 text-indigo-600" /> }
];

const testimonials = [
  { quote: 'LuxeStore delivers the calm, premium feel our customers expect.', name: 'Amelia R.', role: 'Founder, Atelier & Co.' },
  { quote: 'Minimal UI, fast API, and easy admin uploads—best blend of form and function.', name: 'Dylan K.', role: 'Ecommerce Lead' }
];

const pricing = [
  { name: 'Starter', price: '$0', desc: 'Build and preview', items: ['Public catalog', 'Email verification', 'Mock/API toggle'] },
  { name: 'Growth', price: '$29', desc: 'Launch and scale', items: ['Stripe-ready checkout', 'Admin uploads', 'Analytics export'] },
  { name: 'Enterprise', price: 'Let’s talk', desc: 'Custom stack', items: ['SSO & RBAC', 'Custom workflows', 'Priority support'] }
];

const placeholderImage = 'https://placehold.co/600x400';

export function Home() {
  return (
    <div className="space-y-14">
      <section className="overflow-hidden rounded-3xl border border-indigo-100 bg-gradient-to-br from-white via-indigo-50 to-white shadow-sm">
        <div className="grid gap-10 p-8 md:grid-cols-2 md:p-12">
          <div className="flex flex-col justify-center space-y-5">
            <div className="inline-flex items-center gap-2 rounded-full bg-indigo-50 px-3 py-1 text-xs font-semibold text-indigo-700">
              <Sparkle className="h-4 w-4" />
              LuxeStore Experience
            </div>
            <h1 className="text-4xl font-semibold text-gray-900 md:text-5xl leading-tight">Minimal commerce, elevated.</h1>
            <p className="text-lg text-gray-600">
              Havinic-inspired landing with curated products, refined typography, and a calm flow. Built on a secure API with email
              verification and Stripe-ready checkout.
            </p>
            <div className="flex flex-col gap-3 sm:flex-row">
              <Button className="w-full sm:w-auto" onClick={() => (window.location.href = '/products')}>
                View catalog
              </Button>
              <Button variant="secondary" className="w-full sm:w-auto" onClick={() => (window.location.href = '/login')}>
                Sign in
              </Button>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-500">
              <Shield className="h-4 w-4 text-indigo-600" />
              Email verification + JWT auth keep accounts secure
            </div>
          </div>
          <div className="relative">
            <div className="absolute inset-0 -z-10 bg-gradient-to-tr from-indigo-100 via-white to-indigo-50 blur-3xl" />
            <div className="flex h-full items-center justify-center">
              <div className="w-full max-w-md space-y-4 rounded-2xl bg-white p-6 shadow-lg ring-1 ring-gray-100">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-indigo-600 text-white shadow-sm">
                    <ShoppingBag className="h-6 w-6" />
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-gray-900">Featured Drop</p>
                    <p className="text-xs text-gray-500">Handpicked for a clean aesthetic</p>
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
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-3">
        {features.map((item) => (
          <Card key={item.title} className="flex h-full flex-col gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-50">{item.icon}</div>
            <div className="space-y-2">
              <p className="text-base font-semibold text-gray-900">{item.title}</p>
              <p className="text-sm text-gray-600">{item.copy}</p>
            </div>
          </Card>
        ))}
      </section>

      <section className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm font-medium text-indigo-700">Showcase</p>
            <h2 className="text-2xl font-semibold text-gray-900">Featured products</h2>
            <p className="text-sm text-gray-500">Pulled from API when available; falls back to mock data.</p>
          </div>
          <Button variant="secondary" onClick={() => (window.location.href = '/products')}>
            View all
          </Button>
        </div>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {mockProducts.map((product) => (
            <Card key={product.id} className="flex flex-col gap-3 overflow-hidden">
              <div className="relative h-36 w-full overflow-hidden rounded-lg bg-gray-100">
                <img src={product.imageUrl || placeholderImage} alt={product.name} className="h-full w-full object-cover" />
              </div>
              <div>
                <p className="text-base font-semibold text-gray-900">{product.name}</p>
                <p className="text-sm text-gray-500 line-clamp-2">{product.description}</p>
              </div>
              <div className="mt-auto flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500">SKU {product.sku}</p>
                  <p className="text-lg font-semibold text-gray-900">${product.price.toFixed(2)}</p>
                </div>
                <Button
                  className="flex items-center gap-2"
                  onClick={() => {
                    window.location.href = `/products/${product.id}`;
                  }}
                >
                  View
                </Button>
              </div>
            </Card>
          ))}
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-2">
        {testimonials.map((t) => (
          <Card key={t.name} className="space-y-3">
            <div className="flex items-center gap-2 text-indigo-600">
              <Star className="h-4 w-4" />
              <Star className="h-4 w-4" />
              <Star className="h-4 w-4" />
              <Star className="h-4 w-4" />
              <Star className="h-4 w-4" />
            </div>
            <p className="text-lg text-gray-900">{t.quote}</p>
            <p className="text-sm font-semibold text-gray-800">{t.name}</p>
            <p className="text-xs text-gray-500">{t.role}</p>
          </Card>
        ))}
      </section>

      <section className="grid gap-4 md:grid-cols-3">
        {pricing.map((tier) => (
          <Card key={tier.name} className="flex h-full flex-col gap-3">
            <p className="text-sm font-medium text-indigo-700">{tier.name}</p>
            <p className="text-3xl font-semibold text-gray-900">{tier.price}</p>
            <p className="text-sm text-gray-600">{tier.desc}</p>
            <ul className="mt-2 space-y-2 text-sm text-gray-700">
              {tier.items.map((item) => (
                <li key={item} className="flex items-center gap-2">
                  <span className="h-2 w-2 rounded-full bg-indigo-500" /> {item}
                </li>
              ))}
            </ul>
            <Button variant={tier.name === 'Growth' ? 'primary' : 'secondary'} className={cn('mt-auto', tier.name === 'Growth' && 'bg-indigo-600 text-white')}>
              Choose {tier.name}
            </Button>
          </Card>
        ))}
      </section>
    </div>
  );
}
