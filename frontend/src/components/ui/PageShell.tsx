import { PropsWithChildren } from 'react';
import { NavLink } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { ShoppingBag, LogIn, UserPlus, Home, Settings, Sparkles } from 'lucide-react';

export function PageShell({ children }: PropsWithChildren<{}>) {
  return (
    <div className="relative min-h-screen overflow-hidden bg-gradient-to-br from-[hsl(213,100%,97%)] via-white to-[hsl(230,60%,93%)]">
      <div className="pointer-events-none absolute inset-0 -z-10 opacity-70">
        <div className="absolute left-[-10%] top-[-20%] h-80 w-80 rounded-full bg-[radial-gradient(circle_at_center,_rgba(59,130,246,0.16)_0%,_transparent_60%)] blur-3xl" />
        <div className="absolute right-[-12%] top-[-10%] h-96 w-96 rounded-full bg-[radial-gradient(circle_at_center,_rgba(6,182,212,0.16)_0%,_transparent_60%)] blur-3xl" />
      </div>

      <header className="sticky top-0 z-20 border-b border-white/40 bg-white/70 backdrop-blur-xl">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-4">
          <div className="flex items-center gap-3 text-gray-900">
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-br from-blue-600 to-sky-500 text-white shadow-lg shadow-blue-500/20">
              <ShoppingBag className="h-5 w-5" />
            </div>
            <div>
              <p className="text-base font-semibold text-gray-900">LuxeStore</p>
              <p className="text-xs text-gray-500">Calm commerce for modern brands</p>
            </div>
          </div>
          <nav className="flex items-center gap-2 text-sm text-gray-700">
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/home">
              <div className="flex items-center gap-1">
                <Home className="h-4 w-4" />
                Home
              </div>
            </NavLink>
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/store">
              Store
            </NavLink>
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/products">
              Products
            </NavLink>
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/login">
              <div className="flex items-center gap-1">
                <LogIn className="h-4 w-4" />
                Login
              </div>
            </NavLink>
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/register">
              <div className="flex items-center gap-1">
                <UserPlus className="h-4 w-4" />
                Register
              </div>
            </NavLink>
            <NavLink className={({ isActive }) => navClasses(isActive)} to="/admin/products/new">
              <div className="flex items-center gap-1">
                <Settings className="h-4 w-4" />
                Admin
              </div>
            </NavLink>
            <a
              href="/store"
              className="ml-2 hidden items-center gap-2 rounded-full bg-gradient-to-r from-blue-600 to-sky-500 px-4 py-2 text-xs font-semibold text-white shadow-lg shadow-blue-500/25 transition hover:shadow-blue-500/35 sm:flex"
            >
              <Sparkles className="h-4 w-4" /> Start shopping
            </a>
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-12 md:py-14">{children}</main>
    </div>
  );
}

function navClasses(isActive: boolean) {
  return cn(
    'rounded-full px-3 py-2 transition-colors hover:bg-white hover:text-blue-700',
    isActive ? 'bg-white text-blue-700 shadow-sm' : 'text-gray-700'
  );
}
