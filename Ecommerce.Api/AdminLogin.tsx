import { FormEvent, useState } from "react"
import { Link, useLocation, useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"

type LoginResponse = {
  token?: string
  Token?: string
}
const apiBaseUrl = import.meta.env.VITE_API_URL || ""

const AdminLogin = () => {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const navigate = useNavigate()
  const location = useLocation()

  const state = location.state as { from?: string } | null
  const defaultRedirect = location.pathname.startsWith("/admin") ? "/admin" : "/"
  const redirectTo = state?.from ?? defaultRedirect

  const onSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setIsLoading(true)

    try {
      const response = await fetch(`${apiBaseUrl}/api/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(errorText || "Login failed.")
      }

      const data: LoginResponse = await response.json()
      const token = data.token ?? data.Token
      if (!token) {
        throw new Error("Login response did not contain a token.")
      }

      localStorage.setItem("authToken", token)
      toast.success("Signed in successfully.")
      navigate(redirectTo, { replace: true })
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Unable to sign in.")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="mx-auto max-w-md">
      <Card>
        <CardHeader>
          <CardTitle>Sign In</CardTitle>
        </CardHeader>
        <CardContent>
          <form className="space-y-4" onSubmit={onSubmit}>
            <Input
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="Email"
              required
            />
            <Input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Password"
              required
            />
            <Button className="w-full" disabled={isLoading} type="submit">
              {isLoading ? "Signing In..." : "Sign In"}
            </Button>
          </form>
          <p className="mt-4 text-center text-sm text-gray-600">
            Back to <Link className="underline" to="/">store</Link>
          </p>
        </CardContent>
      </Card>
    </div>
  )
}

export default AdminLogin
