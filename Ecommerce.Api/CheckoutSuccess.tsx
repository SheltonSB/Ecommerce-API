import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"

const CheckoutSuccess = () => {
  return (
    <div className="mx-auto max-w-2xl space-y-4 text-center">
      <h1 className="font-display text-4xl">Payment Successful</h1>
      <p className="text-gray-700">Your order was processed successfully.</p>
      <div className="flex justify-center gap-3">
        <Link to="/">
          <Button>Continue Shopping</Button>
        </Link>
        <Link to="/cart">
          <Button variant="outline">View Cart</Button>
        </Link>
      </div>
    </div>
  )
}

export default CheckoutSuccess
