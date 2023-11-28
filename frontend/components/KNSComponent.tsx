"use client";
import { SSX } from "@spruceid/ssx";
import { useCallback, useEffect, useState } from "react";
import KeplerStorageComponent from "./KeplerStorageComponent";
import RebaseCredentialComponent from "./RebaseCredentialComponent";
import SpruceKitCredentialComponent from "./SpruceKitCredentialComponent";
import { ethers } from "ethers";

type Design = { label: string; design: string };
type Designs = Design[];

const REVERSE_RECORDS_ADDRESS = "0x87f4483E4157a6592dd1d1546f145B5EE22c790a";
const REVERSE_RECORDS_ABI = ["function getName(address) view returns (string)"];

const KNSComponent = ({
  unityProvider,
  isLoaded,
  addEventListener,
  removeEventListener,
  sendMessage,
}: {
  unityProvider: any;
  isLoaded: boolean;
  addEventListener: any;
  removeEventListener: any;
  sendMessage: any;
}) => {
  const getENSDetails = useCallback(
    async (address: string) => {
      if (!address) {
        return;
      }

      const provider = new ethers.providers.JsonRpcProvider(
        process.env.NEXT_PUBLIC_KLAYTN_MAINNET_PROVIDER_URL // Klaytn mainnet RPC URL
      );

      let reverseRecords = new ethers.Contract(
        REVERSE_RECORDS_ADDRESS,
        REVERSE_RECORDS_ABI,
        provider
      );

      let ensName = await reverseRecords.getName(address);

      if (!ensName || ensName === "") {
        ensName = address.slice(0, 6) + "..." + address.slice(-4);
      }

      let ensAvatarUrl =
        "https://cdn.pixabay.com/photo/2014/04/02/10/25/man-303792_1280.png";

      console.log("kns", {
        ensName,
        ensAvatarUrl,
      });

      sendMessage(
        "CanvasManager",
        "SetEnsDetails",
        JSON.stringify({
          ensName,
          ensAvatarUrl,
        })
      );
    },
    [unityProvider, isLoaded]
  );

  useEffect(() => {
    // requestFullscreen(true);
    //@ts-ignore
    addEventListener("GetENS", getENSDetails);
    return () => {
      //@ts-ignore
      removeEventListener("GetENS", getENSDetails);
    };
  }, [
    addEventListener,
    removeEventListener,
    getENSDetails,
    unityProvider,
    isLoaded,
  ]);

  return <></>;
};

export default KNSComponent;
