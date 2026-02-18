import { useQuery } from "@tanstack/react-query"
import { Link, useParams } from "react-router-dom"
import { useCart } from "@/CartContext"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"

type ProductDetailResponse = {
  id: number
  name: string
  description?: string | null
  price: number
  stockQuantity: number
  imageUrl?: string | null
}
const apiBaseUrl = import.meta.env.VITE_API_URL || ""

const getProduct = async (id: string): Promise<ProductDetailResponse> => {
  const response = await fetch(`${apiBaseUrl}/api/products/${id}`)
  if (!response.ok) {
    throw new Error("Product not found.")
  }

  return response.json()
}

const ProductDetail = () => {
  const { id } = useParams()
  const { addItem } = useCart()

  const { data: product, isLoading, isError } = useQuery({
    queryKey: ["productDetail", id],
    queryFn: () => getProduct(id ?? ""),
    enabled: Boolean(id),
  })

  if (isLoading) {
    return <p>Loading product...</p>
  }

  if (isError || !product) {
    return (
      <div className="space-y-4">
        <p>Product not found.</p>
        <Link to="/">
          <Button variant="outline">Back to Store</Button>
        </Link>
      </div>
    )
  }

  return (
    <Card>
      <CardContent className="grid grid-cols-1 gap-8 p-6 md:grid-cols-2">
        <img
          src={product.imageUrl || "/placeholder.svg"}
          alt={product.name}
          className="h-80 w-full rounded-md object-cover"
        />
        <div className="space-y-4">
          <h1 className="font-display text-4xl">{product.name}</h1>
          <p className="text-2xl font-semibold">${product.price.toFixed(2)}</p>
          <p className="text-gray-700">{product.description || "No description available."}</p>
          <p className="text-sm text-gray-600">
            {product.stockQuantity > 0 ? `${product.stockQuantity} in stock` : "Out of stock"}
          </p>
          <div className="flex gap-3">
            <Button
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
              Add to Cart
            </Button>
            <Link to="/cart">
              <Button variant="outline">Go to Cart</Button>
            </Link>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

export default ProductDetail
