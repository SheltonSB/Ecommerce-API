import { ShoppingBag, User, LogOut, Menu } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';

const Navbar = () => {
    const navigate = useNavigate();
    const token = localStorage.getItem('token');

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <nav className="sticky top-0 z-50 bg-white/80 backdrop-blur-md border-b border-gray-100">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between h-16 items-center">
                    {/* Logo */}
                    <Link to="/" className="text-2xl font-bold text-gray-900 tracking-tighter flex items-center gap-2">
                        <div className="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <ShoppingBag size={18} />
                        </div>
                        LuxeStore
                    </Link>

                    {/* Actions */}
                    <div className="flex items-center space-x-6">
                        <Link to="/cart" className="relative text-gray-600 hover:text-indigo-600 transition">
                            <ShoppingBag className="w-6 h-6" />
                            {/* Badge placeholder */}
                            <span className="absolute -top-1 -right-1 bg-indigo-600 text-white text-xs w-4 h-4 flex items-center justify-center rounded-full">2</span>
                        </Link>
                        
                        {token ? (
                            <button onClick={handleLogout} className="text-gray-500 hover:text-red-500 transition">
                                <LogOut className="w-6 h-6" />
                            </button>
                        ) : (
                            <Link to="/login" className="flex items-center gap-2 text-gray-600 hover:text-indigo-600 font-medium transition">
                                <User className="w-5 h-5" />
                                <span>Sign In</span>
                            </Link>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;