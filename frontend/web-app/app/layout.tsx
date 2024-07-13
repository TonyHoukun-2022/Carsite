import "./globals.css";
import Navbar from "./nav/Navbar";


export const metadata = {
  title: "Carsites",
  description: "Car Auction app",
};

export default function RootLayout({
  children
}: {
  children: React.ReactNode
}) {
  return (
    <html>
      <body>
        <Navbar />
        <main className="container mx-auto px-5 pt-10">
          {/* app/page.tsx  */}
          {children}
        </main>
      </body>
    </html>
  )
}
