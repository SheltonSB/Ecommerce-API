import { useQuery } from "@tanstack/react-query"
import { Link } from "react-router-dom"
import { useCart } from "@/CartContext"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"

type ProductListItem = {
  id: number
  name: string
  price: number
  sku: string
  stockQuantity: number
  imageUrl?: string | null
  isActive: boolean
  categoryName: string
}

type PagedResponse<T> = {
  items: T[]
}
const apiBaseUrl = import.meta.env.VITE_API_URL || ""

const getProducts = async (): Promise<ProductListItem[]> => {
  const response = await fetch(`${apiBaseUrl}/api/products?page=1&pageSize=24&isActive=true`)
  if (!response.ok) {
    throw new Error("Failed to load products.")
  }

  const payload: ProductListItem[] | PagedResponse<ProductListItem> = await response.json()
  return Array.isArray(payload) ? payload : payload.items
}

const Store = () => {
  const { addItem } = useCart()
  const { data: products = [], isLoading, isError } = useQuery({
    queryKey: ["storeProducts"],
    queryFn: getProducts,
  })

  if (isLoading) {
    return <p>Loading products...</p>
  }

  if (isError) {
    return <p>Could not load products. Check your API URL and backend status.</p>
  }

  return (
    <section className="space-y-6">
      <div className="flex items-end justify-between">
        <div>
          <h1 className="font-display text-4xl">Store</h1>
          <p className="text-sm text-gray-600">Browse and add items to cart.</p>
        </div>
        <p className="text-sm text-gray-600">{products.length} products</p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-3">
        {products.map((product) => (
          <Card key={product.id} className="flex h-full flex-col">
            <CardHeader>
              <img
                src={product.imageUrl || "/placeholder.svg"}
                alt={product.name}
                className="h-48 w-full rounded-md object-cover"
              />
              <CardTitle className="pt-2 text-xl">{product.name}</CardTitle>
              <p className="text-xs uppercase tracking-wide text-gray-500">{product.categoryName}</p>
            </CardHeader>
            <CardContent className="mt-auto space-y-3">
              <p className="text-lg font-semibold">${product.price.toFixed(2)}</p>
              <div className="flex gap-2">
                <Link className="flex-1" to={`/products/${product.id}`}>
                  <Button variant="outline" className="w-full">
                    View
                  </Button>
                </Link>
                <Button
                  className="flex-1"
                  disabled={product.stockQuantity <= 0}
                  onClick={() =>
                    addItem(
                      {
                        id: product.id,
                        name: product.name,
                        price: product.price,
                        imageUrl: product.imageUrl ?? undefined,
                      },
                      1
                    )
                  }
                >
                  {product.stockQuantity > 0 ? "Add to Cart" : "Out of Stock"}
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </section>
  )
}

export default Store
