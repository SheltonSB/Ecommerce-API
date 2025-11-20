import { ShoppingCart } from 'lucide-react';

interface ProductProps {
    id: number;
    name: string;
    price: number;
    description: string;
    // imageUrl: string; // Uncomment when you have images
}

const ProductCard = ({ product }: { product: ProductProps }) => {
    return (
        <div className="group bg-white rounded-2xl border border-gray-100 overflow-hidden hover:shadow-xl hover:shadow-indigo-100 transition-all duration-300">
            {/* Image Placeholder */}
            <div className="h-64 bg-gray-100 relative overflow-hidden">
                <div className="absolute inset-0 bg-gradient-to-tr from-gray-200 to-gray-50 group-hover:scale-105 transition-transform duration-500" />
                <span className="absolute top-4 left-4 bg-white/90 backdrop-blur px-3 py-1 text-xs font-bold uppercase tracking-wide rounded-full">New</span>
            </div>

            <div className="p-6">
                <div className="flex justify-between items-start mb-2">
                    <h3 className="text-lg font-semibold text-gray-900 group-hover:text-indigo-600 transition-colors">{product.name}</h3>
                    <span className="text-lg font-bold text-gray-900">${product.price}</span>
                </div>
                <p className="text-gray-500 text-sm mb-4 line-clamp-2">{product.description}</p>
                
                <button className="w-full bg-gray-900 text-white py-3 rounded-xl font-medium flex items-center justify-center gap-2 group-hover:bg-indigo-600 transition-colors">
                    <ShoppingCart size={18} />
                    Add to Cart
                </button>
            </div>
        </div>
    );
};

export default ProductCard;