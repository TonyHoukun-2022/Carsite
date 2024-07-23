'use server'

import { Auction, PageResult } from '@/types'
import { fetchWrapper } from '@/lib/fetchWrapper'
import { FieldValues } from 'react-hook-form'
import { revalidatePath } from 'next/cache'

export async function getData(query: string): Promise<PageResult<Auction>> {
  return await fetchWrapper.get(`search${query}`)
}

//test
export async function updateAuctionTest() {
  const data = {
    mileage: Math.floor(Math.random() * 100000) + 1,
  }

  return await fetchWrapper.put(`auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c`, data)
}

export async function createAuction(data: FieldValues) {
  return await fetchWrapper.post('auctions', data)
}

export const getAuctionById = async (id: string): Promise<Auction> => {
  return await fetchWrapper.get(`auctions/${id}`)
}

export const updateAuction = async (data: FieldValues, id: string) => {
  const res = fetchWrapper.put(`auctions/${id}`, data)
  revalidatePath(`/auctions/${id}`)
  return res
}

export const deleteAuction = async (id: string) => {
  return await fetchWrapper.del(`auctions/${id}`)
}
