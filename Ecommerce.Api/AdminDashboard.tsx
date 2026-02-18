import { useQuery } from "@tanstack/react-query"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type ProductSummaryResponse = {
  totalItems?: number
}
const apiBaseUrl = import.meta.env.VITE_API_URL || ""

const getDashboardSummary = async (): Promise<ProductSummaryResponse> => {
  const response = await fetch(`${apiBaseUrl}/api/products?page=1&pageSize=1`, {
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("authToken")}`,
    },
  })

  if (!response.ok) {
    throw new Error("Failed to load dashboard data.")
  }

  return response.json()
}

const AdminDashboard = () => {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["adminDashboardSummary"],
    queryFn: getDashboardSummary,
  })

  if (isLoading) {
    return <p>Loading dashboard...</p>
  }

  if (isError) {
    return <p>Dashboard data could not be loaded.</p>
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold">Overview</h2>
      <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Total Products</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-semibold">{data?.totalItems ?? 0}</p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default AdminDashboard
