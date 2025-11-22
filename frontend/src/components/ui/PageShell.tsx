import { PropsWithChildren } from 'react';
import { NavLink } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { ShoppingBag, LogIn, UserPlus, Home, Settings } from 'lucide-react';

export function PageShell({ children }: PropsWithChildren<{}>) {
  return (
    <div className="min-h-screen bg-gray-50">
      <div className="absolute inset-0 -z-10 bg-gradient-to-br from-indigo-50 via-white to-gray-50" />
      <header className="border-b border-gray-100 bg-white/80 backdrop-blur">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-4">
          <div className="flex items-center gap-3 text-gray-900">
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-indigo-600 text-white shadow-sm">
              <ShoppingBag className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm font-semibold text-gray-900">LuxeStore</p>
              <p className="text-xs text-gray-500">Modern minimal commerce</p>
            </div>
          </div>
          <nav className="flex items-center gap-2 text-sm text-gray-600">
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
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-6xl px-4 py-10">{children}</main>
    </div>
  );
}

function navClasses(isActive: boolean) {
  return cn(
    'rounded-md px-3 py-2 transition-colors hover:bg-accent/40',
    isActive ? 'text-indigo-700 bg-indigo-50' : 'text-gray-600'
  );
}
