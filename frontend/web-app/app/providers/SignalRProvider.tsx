'use client'

import { useAuctionStore } from '@/hooks/useAuctionStore';
import { useBidStore } from '@/hooks/useBidStore';
import { Auction, AuctionFinished, Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { User } from 'next-auth';
import React, { ReactNode, useEffect, useState } from 'react'
import { toast } from 'react-hot-toast';
import { getAuctionById } from '../apis/auctionApi';
import AuctionCreatedToast from '../components/AuctionCreatedToast';
import AuctionFinishedToast from '../components/AuctionFinishedToast';

type Props = {
  children: ReactNode
  user: User | null
}

export default function SignalRProvider({ children, user }: Props) {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);
  const addBid = useBidStore(state => state.addBid);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      //gateway-svc/notification
      .withUrl('http://localhost:6001/notifications')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('Connected to notification signalR hub');

          //receive from notification-svc 
          connection.on('BidPlaced', (bid: Bid) => {
            if (bid.bidStatus.includes('Accepted')) {
              setCurrentPrice(bid.auctionId, bid.amount);
            }
            addBid(bid);
          });

          connection.on('AuctionCreated', (auction: Auction) => {
            if (user?.username !== auction.seller) {
              return toast(
                <AuctionCreatedToast auction={auction} />,
                { duration: 10000 }
              )
            }
          });

          connection.on('AuctionFinished', (finishedAuction: AuctionFinished) => {
            const auction = getAuctionById(finishedAuction.auctionId);

            return toast.promise(auction, {
              loading: 'Loading',
              success: (auction) =>
                <AuctionFinishedToast
                  finishedAuction={finishedAuction}
                  auction={auction}
                />,
              error: (err) => 'Auction finished!'
            }, { success: { duration: 10000, icon: null } })
          })


        })
        .catch(error => console.log(error));
    }

    // The useEffect hook returns a cleanup function that stops the SignalR connection when the component unmounts or when the connection changes.
    // return () => { connection?.stop(); }: Ensures the connection is properly stopped to avoid memory leaks.
    return () => {
      connection?.stop();
    }
  }, [connection, setCurrentPrice])

  return (
    children
  )
}