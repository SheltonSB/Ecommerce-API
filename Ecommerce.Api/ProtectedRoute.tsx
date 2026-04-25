import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  // In a real app, you'd have a proper auth hook: const { isAuthenticated } = useAuth();
  const isAuthenticated = !!localStorage.getItem("authToken");

  if (!isAuthenticated) {
    // Redirect them to the /admin/login page, but save the current location they were
    // trying to go to when they were redirected. This allows us to send them
    // along to that page after they login, which is a nicer user experience.
    return <Navigate to="/admin/login" replace />;
  }

  return children;
};

export default ProtectedRoute;