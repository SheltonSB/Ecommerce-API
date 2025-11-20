import { useEffect, useState } from 'react';
import api from '../api/axios';
import ProductCard from '../components/ProductCard.tsx';
import { ArrowRight } from 'lucide-react';

const Home = () => {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        // Fetch products from your .NET API
        api.get('/Products').then(res => {
            setProducts(res.data.items || []);
        }).catch(err => console.error(err));
    }, []);

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Hero Section */}
            <div className="bg-white border-b border-gray-100">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24 flex flex-col items-center text-center">
                    <h1 className="text-5xl md:text-7xl font-bold text-gray-900 tracking-tight mb-6">
                        Minimalist.<br/>
                        <span className="text-indigo-600">Modern.</span> You.
                    </h1>
                    <p className="text-xl text-gray-500 max-w-2xl mb-10">
                        Discover our curated collection of premium essentials designed for the modern lifestyle. Quality that speaks for itself.
                    </p>
                    <button className="bg-gray-900 text-white px-8 py-4 rounded-full font-medium text-lg hover:bg-indigo-600 transition-colors flex items-center gap-2">
                        Shop Collection <ArrowRight size={20} />
                    </button>
                </div>
            </div>

            {/* Product Grid */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
                <div className="flex justify-between items-end mb-8">
                    <h2 className="text-2xl font-bold text-gray-900">Featured Products</h2>
                    <a href="#" className="text-indigo-600 font-medium hover:text-indigo-700">View all</a>
                </div>
                
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
                    {products.map((product: any) => (
                        <ProductCard key={product.id} product={product} />
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Home;