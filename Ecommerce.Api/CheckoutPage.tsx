import { useCart } from "@/CartContext";
import { Navigate, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
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
const apiBaseUrl = import.meta.env.VITE_API_URL || "";
type CheckoutFormValues = {
  name: string;
  email: string;
  address: string;
};

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
  const authToken = localStorage.getItem("authToken");
  const response = await fetch(`${apiBaseUrl}/api/checkout`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(authToken ? { Authorization: `Bearer ${authToken}` } : {})
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
  const navigate = useNavigate();

  const { register, handleSubmit, formState: { errors } } = useForm<CheckoutFormValues>();

  const checkoutMutation = useMutation({
    mutationFn: createCheckoutSession,
    onSuccess: async (data) => {
      toast.success("Redirecting to payment...");

      try {
        await redirectToStripeCheckout(data);
      } catch {
        // Fallback for any Stripe.js load/runtime failure
        if (data.url) {
          window.location.href = data.url;
          return;
        }

        toast.error("Unable to redirect to Stripe checkout.");
      }
    },
    onError: () => {
      toast.error("There was an issue processing your order. Please try again.");
    },
  });

  const onSubmit = (_data: CheckoutFormValues) => {
    const authToken = localStorage.getItem("authToken");
    if (!authToken) {
      toast.error("Please sign in before checking out.");
      navigate("/login", { state: { from: "/checkout" } });
      return;
    }

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
          <Input
            placeholder="Full Name"
            {...register("name", {
              required: "Name is required",
              minLength: { value: 2, message: "Name is required" },
            })}
          />
          {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
          
          <Input
            placeholder="Email Address"
            {...register("email", {
              required: "Email is required",
              pattern: {
                value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: "Invalid email address",
              },
            })}
          />
          {errors.email && <p className="text-red-500 text-sm">{errors.email.message}</p>}

          <Input
            placeholder="Shipping Address"
            {...register("address", {
              required: "Address is required",
              minLength: { value: 5, message: "Address is required" },
            })}
          />
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
