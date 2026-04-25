import { Link, useNavigate } from "react-router-dom"
import { useCart } from "@/CartContext"
import { Button } from "@/components/ui/button"

const Header = () => {
  const { totalItems } = useCart()
  const navigate = useNavigate()
  const isAuthenticated = Boolean(localStorage.getItem("authToken"))

  const handleLogout = () => {
    localStorage.removeItem("authToken")
    navigate("/")
  }

  return (
    <header className="border-b border-border bg-white">
      <div className="container mx-auto flex items-center justify-between px-4 py-4">
        <Link to="/" className="font-display text-2xl font-semibold tracking-wide">
          LuxeStore
        </Link>
        <nav className="flex items-center gap-3">
          <Link to="/" className="text-sm hover:underline">
            Shop
          </Link>
          <Link to="/cart" className="text-sm hover:underline">
            Cart ({totalItems})
          </Link>
          <Link to="/checkout" className="text-sm hover:underline">
            Checkout
          </Link>
          {isAuthenticated ? (
            <>
              <Link to="/admin" className="text-sm hover:underline">
                Admin
              </Link>
              <Button size="sm" variant="outline" onClick={handleLogout}>
                Logout
              </Button>
            </>
          ) : (
            <Link to="/login" className="text-sm hover:underline">
              Sign In
            </Link>
          )}
        </nav>
      </div>
    </header>
  )
}

export default Header
