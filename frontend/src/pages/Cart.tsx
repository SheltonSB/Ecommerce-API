import { useState } from 'react';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { ShoppingCart, Trash2 } from 'lucide-react';

type CartItem = {
  id: number;
  name: string;
  price: number;
  quantity: number;
};

export function Cart() {
  const [items, setItems] = useState<CartItem[]>([
    { id: 1, name: 'Artisan Leather Tote', price: 249, quantity: 1 },
    { id: 2, name: 'Minimal Desk Lamp', price: 159, quantity: 2 }
  ]);

  const updateQuantity = (id: number, qty: number) => {
    setItems((prev) =>
      prev.map((item) => (item.id === id ? { ...item, quantity: Math.max(1, qty) } : item))
    );
  };

  const removeItem = (id: number) => {
    setItems((prev) => prev.filter((item) => item.id !== id));
  };

  const subtotal = items.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <div className="grid gap-6 lg:grid-cols-3">
      <div className="lg:col-span-2 space-y-3">
        {items.map((item) => (
          <Card key={item.id} className="flex items-center justify-between">
            <div>
              <p className="text-base font-semibold text-gray-900">{item.name}</p>
              <p className="text-sm text-gray-500">${item.price.toFixed(2)}</p>
            </div>
            <div className="flex items-center gap-3">
              <div className="flex items-center gap-2">
                <button className="rounded border px-2" onClick={() => updateQuantity(item.id, item.quantity - 1)}>
                  -
                </button>
                <span className="w-8 text-center text-sm">{item.quantity}</span>
                <button className="rounded border px-2" onClick={() => updateQuantity(item.id, item.quantity + 1)}>
                  +
                </button>
              </div>
              <Button variant="secondary" onClick={() => removeItem(item.id)}>
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          </Card>
        ))}
      </div>
      <Card className="space-y-3">
        <div className="flex items-center gap-2 text-indigo-700">
          <ShoppingCart className="h-5 w-5" />
          <p className="text-sm font-semibold">Order Summary</p>
        </div>
        <div className="flex justify-between text-sm text-gray-700">
          <span>Subtotal</span>
          <span className="font-semibold text-gray-900">${subtotal.toFixed(2)}</span>
        </div>
        <Button className="w-full">Proceed to Checkout</Button>
      </Card>
    </div>
  );
}
