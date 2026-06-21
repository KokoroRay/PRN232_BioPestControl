import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useToast } from '../context/ToastContext';
import type { Product } from '../types/catalog';

export function useAddToCart() {
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const { showToast } = useToast();

  const handleAddToCart = async (
    product: Pick<Product, 'id' | 'name' | 'unitPrice' | 'imageUrl'>,
    options?: { quantity?: number; buyNow?: boolean },
  ) => {
    const quantity = options?.quantity ?? 1;
    const result = await addToCart(product, quantity);

    if (result.needLogin) {
      showToast(result.message ?? 'Please login to add items to cart', 'error');
      navigate('/login', { state: { from: window.location.pathname } });
      return false;
    }

    if (!result.success) {
      showToast(result.message ?? 'Could not add to cart', 'error');
      return false;
    }

    if (options?.buyNow) {
      try {
        localStorage.setItem('lastBuyProductId', String(product.id));
      } catch {
        // ignore
      }
      navigate('/cart');
      return true;
    }

    showToast(result.message ?? 'Added to cart');
    return true;
  };

  return { handleAddToCart };
}
