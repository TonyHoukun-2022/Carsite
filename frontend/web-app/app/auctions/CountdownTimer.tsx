// client side render for timer function
'use client'

import React from 'react'
import Countdown, { zeroPad } from 'react-countdown';

type timeFrame = {
  days: number
  hours: number
  minutes: number
  seconds: number
  completed: boolean
}

type Props = {
  auctionEnd: string
}

const getBackgroundColor = (completed: boolean, days: number, hours: number): string => {
  if (completed) {
    return 'bg-red-600';
  } else if (days === 0 && hours < 10) {
    return 'bg-amber-600';
  } else {
    return 'bg-green-600';
  }
};

const renderer = ({ days, hours, minutes, seconds, completed }: timeFrame) => {
  return (
    <div className={`
      border-2 
      border-white 
      text-white 
      py-1 
      px-2 
      rounded-lg 
      flex 
      justify-center
      ${getBackgroundColor(completed, days, hours)}
  `}>
      {completed ? (
        <span>Auction finished</span>
      ) : (
        //  suppress warnings related to content mismatch between server-rendered HTML and client-rendered HTML during hydration. When using suppressHydrationWarning, you can prevent these warnings if you know the content will differ between server and client renders, such as when displaying time or dynamic content.
        <span suppressHydrationWarning={true}>
          {zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
        </span>
      )}
    </div>
  )
};

export default function CountdownTimer({ auctionEnd }: Props) {
  return (
    <div>
      <Countdown date={auctionEnd} renderer={renderer} />
    </div>
  )
}
