import { Toaster as Sonner, type ToasterProps } from "sonner"

const Toaster = ({ ...props }: ToasterProps) => (
  <Sonner richColors position="top-right" closeButton {...props} />
)

export { Toaster }
