import { Link, useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"

const AdminSidebar = () => {
  const navigate = useNavigate()

  const handleLogout = () => {
    localStorage.removeItem("authToken")
    navigate("/admin/login")
  }

  return (
    <aside className="w-64 border-r border-border bg-white p-4">
      <h2 className="mb-6 font-display text-2xl font-semibold">Admin</h2>
      <nav className="flex flex-col gap-2">
        <Link className="rounded px-3 py-2 hover:bg-gray-100" to="/admin">
          Dashboard
        </Link>
        <Link className="rounded px-3 py-2 hover:bg-gray-100" to="/admin/products">
          Products
        </Link>
      </nav>
      <div className="mt-8">
        <Button className="w-full" variant="outline" onClick={handleLogout}>
          Logout
        </Button>
      </div>
    </aside>
  )
}

export default AdminSidebar
