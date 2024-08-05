import { getCurrentUser } from "./apis/authApi";
import "./globals.css";
import Navbar from "./nav/Navbar";
import SignalRProvider from "./providers/SignalRProvider";
import ToasterProvider from "./providers/ToasterProvider";


export const metadata = {
  title: "Carsites",
  description: "Car Auction app",
};

export default async function RootLayout({
  children
}: {
  children: React.ReactNode
}) {
  const user = await getCurrentUser()
  return (
    <html>
      <body>
        <ToasterProvider />
        <Navbar />
        <main className="container mx-auto px-5 pt-10">
          <SignalRProvider user={user}>
            {/* app/page.tsx  */}
            {children}
          </SignalRProvider>

        </main>
      </body>
    </html>
  )
}
