import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { useCart } from "@/contexts/CartContext";
import { Trash2 } from "lucide-react";

const CartPage = () => {
  const { items, totalItems, totalPrice, updateQuantity, removeItem } = useCart();

  return (
    <div className="space-y-8">
      <h1 className="font-display text-4xl">Your Cart</h1>
      {totalItems > 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="md:col-span-2 space-y-4">
            {items.map((item) => (
              <Card key={item.id} className="flex items-center p-4">
                <img src={item.imageUrl || "/placeholder.svg"} alt={item.name} className="w-24 h-24 object-cover rounded-md mr-4" />
                <div className="flex-grow">
                  <h3 className="font-semibold">{item.name}</h3>
                  <p className="text-sm text-gray-600">${item.price.toFixed(2)}</p>
                </div>
                <div className="flex items-center space-x-4">
                  <Input
                    type="number"
                    min="1"
                    value={item.quantity}
                    onChange={(e) => updateQuantity(item.id, parseInt(e.target.value, 10))}
                    className="w-20"
                  />
                  <Button variant="ghost" size="icon" onClick={() => removeItem(item.id)}>
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </Card>
            ))}
          </div>
          <div className="md:col-span-1">
            <Card>
              <CardHeader>
                <CardTitle>Order Summary</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex justify-between">
                  <span>Subtotal ({totalItems} items)</span>
                  <span>${totalPrice.toFixed(2)}</span>
                </div>
                <div className="flex justify-between font-bold text-lg">
                  <span>Total</span>
                  <span>${totalPrice.toFixed(2)}</span>
                </div>
                <Link to="/checkout" className="w-full">
                  <Button className="w-full">
                    Proceed to Checkout
                  </Button>
                </Link>
              </CardContent>
            </Card>
          </div>
        </div>
      ) : (
        <div className="text-center py-16">
          <h2 className="text-2xl font-semibold mb-4">Your cart is empty.</h2>
          <Link to="/">
            <Button>Continue Shopping</Button>
          </Link>
        </div>
      )}
    </div>
  );
};

export default CartPage;