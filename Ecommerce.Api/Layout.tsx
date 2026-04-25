import { Outlet } from "react-router-dom";
import Header from "./Header";

const Layout = () => {
  return (
    <div className="min-h-screen bg-background text-primary">
      <Header />
      <main className="container mx-auto px-4 py-8">
        <Outlet />
      </main>
      {/* A Footer component would go here */}
    </div>
  );
};

export default Layout;