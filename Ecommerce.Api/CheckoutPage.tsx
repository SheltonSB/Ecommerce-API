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

type CheckoutResponse = {
  url: string;
  sessionId: string;
  publishableKey: string;
};

type StripeClient = {
  redirectToCheckout: (options: { sessionId: string }) => Promise<{ error?: { message?: string } }>;
};

declare global {
  interface Window {
    Stripe?: (publishableKey: string) => StripeClient;
  }
}

let stripeJsLoadPromise: Promise<void> | null = null;

const checkoutSchema = z.object({
  name: z.string().min(2, "Name is required"),
  email: z.string().email("Invalid email address"),
  address: z.string().min(5, "Address is required"),
});

type CheckoutFormValues = z.infer<typeof checkoutSchema>;

const loadStripeJs = async () => {
  if (window.Stripe) return;

  if (!stripeJsLoadPromise) {
    stripeJsLoadPromise = new Promise<void>((resolve, reject) => {
      const script = document.createElement("script");
      script.src = "https://js.stripe.com/v3";
      script.async = true;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error("Failed to load Stripe.js."));
      document.head.appendChild(script);
    });
  }

  await stripeJsLoadPromise;
};

const createCheckoutSession = async (cartItems: { productId: number; quantity: number }[]): Promise<CheckoutResponse> => {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/checkout`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      // The auth token would be dynamically retrieved from an auth context/hook
      Authorization: `Bearer ${localStorage.getItem("authToken")}`
    },
    body: JSON.stringify({ items: cartItems }),
  });

  if (!response.ok) {
    throw new Error("Failed to create checkout session.");
  }

  return response.json();
};

const redirectToStripeCheckout = async (data: CheckoutResponse) => {
  await loadStripeJs();

  if (!window.Stripe) {
    throw new Error("Stripe.js failed to initialize.");
  }

  const stripe = window.Stripe(data.publishableKey);
  const result = await stripe.redirectToCheckout({ sessionId: data.sessionId });

  if (result.error?.message) {
    throw new Error(result.error.message);
  }
};

const CheckoutPage = () => {
  const { items, totalPrice, totalItems } = useCart();

  const { register, handleSubmit, formState: { errors } } = useForm<CheckoutFormValues>({
    resolver: zodResolver(checkoutSchema),
  });

  const checkoutMutation = useMutation({
    mutationFn: createCheckoutSession,
    onSuccess: async (data) => {
      toast.success("Redirecting to payment...");

      try {
        await redirectToStripeCheckout(data);
      } catch {
        // Fallback for any Stripe.js load/runtime failure
        window.location.href = data.url;
      }
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
