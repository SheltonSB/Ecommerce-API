# LuxeStore Master Prompt (Spec Sheet)

Use this prompt to brief an AI or developer to build the LuxeStore experience to the desired standard.

## Role & Goal
- **Role:** Senior Frontend Developer & UI/UX Designer.
- **Goal:** Build a modern, luxury aesthetic e-commerce + admin dashboard called **LuxeStore**. Headless UI components, fully responsive.

## Tech Stack
- **React 18+ with TypeScript**, **Vite** (with `@vitejs/plugin-react-swc`).
- **Tailwind CSS 3.4+**, utility-first.
- **Radix UI** primitives for a11y + logic (headless).
- **Icons:** Lucide React.
- **Routing:** React Router DOM v6.
- **State:** React Query v5 (TanStack) for server state; React Context for cart.
- **Forms/Validation:** React Hook Form + Zod.
- **Backend integration:** Supabase client (`VITE_SUPABASE_URL`, `VITE_SUPABASE_PUBLISHABLE_KEY`).
- **Utilities:** clsx + tailwind-merge.

## Design System & Theming
- **Fonts:** Cormorant Garamond for headings (luxury), Inter/system for body.
- **Colors (HSL vars in `src/index.css`):**
  - Primary: `25 25% 20%`
  - Background: `35 20% 97%`
  - Accent: `30 40% 55%`
  - Border: `35 15% 88%`
- **Radius:** ~4px (0.25rem).
- **Animation:** `transition-smooth` using `all 0.4s cubic-bezier(0.4, 0, 0.2, 1)`; use `tailwindcss-animate`.

## UI Architecture (Shadcn pattern)
- Build reusable primitives in `src/components/ui/` wrapping Radix: Button (cva variants: default/outline/ghost/link), Sheet (cart slide-over), Toast (sonner), Carousel (embla), Dialog, Select, Input, Textarea, Table, Card.

## Features
- **Public Storefront (`/store` / `/`):**
  - Sticky header with logo/nav (Products, Collections, About) and actions (Search, Admin Login, Cart).
  - Hero: “Timeless Essentials” with subtle gradient.
  - Product grid of ProductCard (hover zoom + reveal Add to Cart). Data: image, name, category, price.
  - Cart sheet (slide-over) with quantities, totals, removal.
- **Product Details (`/products/:id`):**
  - Two-column layout: gallery (main + thumbs), info (title, price, description, Add to Cart). Toast on add.
- **Admin Dashboard (`/admin`):**
  - Widgets: stats cards (total products, upload shortcut, analytics summary with recharts).
  - Quick actions: Add Product, Manage Inventory. Nav to subpages.
- **Product Management (`/admin/products`):**
  - Table: Image, Name, Category, Price, Stock, Actions (Edit/Delete).
  - Add Product modal (Dialog): name, price, category, stock, description, file upload area (visual).

## Data & Logic
- **Cart Context:** Provide items, addItem, removeItem, updateQuantity, totalItems, totalPrice.
- **Supabase Client:** `src/integrations/supabase/client.ts` singleton for all DB calls.
- **Mock Data:** Use static arrays for development until Supabase is live.

## File Structure (required)
```
src/
├─ components/
│  ├─ ui/          # Shadcn-style primitives (Button, Input, Sheet, etc.)
│  ├─ store/       # Store-specific (ProductCard, CartSheet)
├─ contexts/       # CartContext, etc.
├─ hooks/          # use-toast, use-mobile, etc.
├─ integrations/   # Supabase client
├─ lib/            # utils (cn helper, constants)
├─ pages/          # Store, ProductDetail, Admin, AdminProducts
├─ App.tsx         # Router + providers
└─ main.tsx        # Entry point
```

## Build Tooling
- Tailwind configured to include shadcn UI tokens and HSL variables.
- Use Radix + headless patterns; avoid pre-built theme kits (no MUI/Bootstrap).
- Ensure full responsiveness (mobile-first) and accessible semantics.
