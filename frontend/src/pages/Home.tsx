import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { ShoppingBag, Shield, Truck, Sparkles, Sparkle, Star, ArrowRight, BadgeCheck } from 'lucide-react';
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
      <section className="overflow-hidden rounded-3xl border border-white/60 bg-gradient-to-br from-[hsl(213,100%,97%)] via-white to-[hsl(230,60%,93%)] p-2 shadow-[0_20px_80px_-40px_rgba(15,23,42,0.5)]">
        <div className="grid gap-10 rounded-2xl bg-white/60 p-8 backdrop-blur-xl md:grid-cols-2 md:p-12">
          <div className="flex flex-col justify-center space-y-6">
            <div className="inline-flex items-center gap-2 rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold text-blue-700 ring-1 ring-blue-100">
              <Sparkle className="h-4 w-4" />
              Calm, confident commerce
            </div>
            <h1 className="text-4xl leading-tight text-slate-900 md:text-5xl">Luxe storefronts with real momentum.</h1>
            <p className="text-lg text-slate-600">
              A sapphire-forward system with bold typography, curated product grid, and a secure ASP.NET Core backend—email verification,
              JWT auth, Stripe-ready checkout, and admin uploads.
            </p>
            <div className="flex flex-col gap-3 sm:flex-row">
              <Button className="w-full sm:w-auto" onClick={() => (window.location.href = '/store')}>
                Explore the store <ArrowRight className="h-4 w-4" />
              </Button>
              <Button variant="secondary" className="w-full sm:w-auto" onClick={() => (window.location.href = '/login')}>
                Sign in / Admin
              </Button>
            </div>
            <div className="flex flex-wrap items-center gap-3 text-sm text-slate-500">
              <BadgeCheck className="h-4 w-4 text-blue-600" /> Email verification & JWT auth
              <span className="h-1 w-1 rounded-full bg-slate-300" />
              <Shield className="h-4 w-4 text-blue-600" /> Stripe-ready checkout
              <span className="h-1 w-1 rounded-full bg-slate-300" />
              <Truck className="h-4 w-4 text-blue-600" /> Admin image uploads
            </div>
          </div>
          <div className="relative">
            <div className="absolute inset-0 -z-10 bg-[radial-gradient(circle_at_30%_20%,rgba(59,130,246,0.18),transparent_40%),radial-gradient(circle_at_80%_0%,rgba(6,182,212,0.18),transparent_35%)]" />
            <div className="flex h-full items-center justify-center">
              <div className="glass-panel w-full max-w-md space-y-4 rounded-2xl p-6">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-gradient-to-br from-blue-600 to-sky-500 text-white shadow-md shadow-blue-500/20">
                    <ShoppingBag className="h-6 w-6" />
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-slate-900">Featured Drop</p>
                    <p className="text-xs text-slate-500">Handpicked minimal staples</p>
                  </div>
                </div>
                <div className="space-y-3">
                  {mockProducts.slice(0, 3).map((p) => (
                    <div key={p.id} className="flex items-center justify-between rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-700">
                      <span>{p.name}</span>
                      <span className="font-semibold text-slate-900">${p.price.toFixed(2)}</span>
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
