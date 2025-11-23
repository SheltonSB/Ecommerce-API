import { useCart } from "@/contexts/CartContext";
import { Navigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

const checkoutSchema = z.object({
  name: z.string().min(2, "Name is required"),
  email: z.string().email("Invalid email address"),
  address: z.string().min(5, "Address is required"),
});

type CheckoutFormValues = z.infer<typeof checkoutSchema>;

// This function simulates calling our backend API
const createCheckoutSession = async (cartItems: { productId: number; quantity: number }[]) => {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/checkout`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      // The auth token would be dynamically retrieved from an auth context/hook
      'Authorization': `Bearer ${localStorage.getItem("authToken")}`
    },
    body: JSON.stringify({ items: cartItems }),
  });

  if (!response.ok) {
    throw new Error("Failed to create checkout session.");
  }

  return response.json();
};

const CheckoutPage = () => {
  const { items, totalPrice, totalItems } = useCart();

  const { register, handleSubmit, formState: { errors } } = useForm<CheckoutFormValues>({
    resolver: zodResolver(checkoutSchema),
  });

  const checkoutMutation = useMutation({
    mutationFn: createCheckoutSession,
    onSuccess: (data) => {
      toast.success("Redirecting to payment...");
      // Redirect to the Stripe URL returned by the backend
      window.location.href = data.url;
    },
    onError: () => {
      toast.error("There was an issue processing your order. Please try again.");
    },
  });

  const onSubmit = (data: CheckoutFormValues) => {
    console.log("Customer Info:", data);
    const cartItems = items.map(item => ({ productId: item.id, quantity: item.quantity }));
    checkoutMutation.mutate(cartItems);
  };

  if (totalItems === 0 && !checkoutMutation.isPending) {
    return <Navigate to="/" replace />;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-12">
      <div>
        <h1 className="font-display text-3xl mb-6">Contact & Shipping</h1>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input placeholder="Full Name" {...register("name")} />
          {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
          
          <Input placeholder="Email Address" {...register("email")} />
          {errors.email && <p className="text-red-500 text-sm">{errors.email.message}</p>}

          <Input placeholder="Shipping Address" {...register("address")} />
          {errors.address && <p className="text-red-500 text-sm">{errors.address.message}</p>}

          <Button type="submit" className="w-full" disabled={checkoutMutation.isPending}>
            {checkoutMutation.isPending ? "Processing..." : "Place Order & Pay"}
          </Button>
        </form>
      </div>
      <div>
        <Card>
          <CardHeader>
            <CardTitle>Order Summary</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {items.map(item => (
              <div key={item.id} className="flex justify-between text-sm">
                <span>{item.name} x {item.quantity}</span>
                <span>${(item.price * item.quantity).toFixed(2)}</span>
              </div>
            ))}
            <hr className="my-2" />
            <div className="flex justify-between font-bold text-lg">
              <span>Total</span>
              <span>${totalPrice.toFixed(2)}</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default CheckoutPage;