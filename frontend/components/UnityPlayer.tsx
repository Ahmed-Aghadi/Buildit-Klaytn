"use client";
import { useEffect, useRef, useState } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Script from "next/script";
import SSXComponent from "@/components/SSXComponent";
import ENSComponent from "@/components/ENSComponent";

type Design = { label: string; design: string };
type Designs = Design[];

const UnityPlayer = () => {
  const [loaded, setLoaded] = useState(false);
  const canvasRef = useRef(null);

  const {
    unityProvider,
    addEventListener,
    removeEventListener,
    requestFullscreen,
    sendMessage,
  } = useUnityContext({
    loaderUrl: "Build/Build.loader.js",
    dataUrl: "Build/Build.data",
    frameworkUrl: "Build/Build.framework.js",
    codeUrl: "Build/Build.wasm",
  });

  return (
    <>
      <Script
        src="lib/thirdweb-unity-bridge.js"
        strategy="lazyOnload"
        onLoad={() => {
          console.log(`script loaded correctly, window.FB has been populated`);
          setLoaded(true);
        }}
      />
      {loaded && (
        <>
          <Unity
            unityProvider={unityProvider}
            ref={canvasRef}
            style={{ height: "100dvh", width: "100dvw" }}
            devicePixelRatio={window.devicePixelRatio}
          />
          <SSXComponent
            addEventListener={addEventListener}
            removeEventListener={removeEventListener}
            sendMessage={sendMessage}
          />
          <ENSComponent
            addEventListener={addEventListener}
            removeEventListener={removeEventListener}
            sendMessage={sendMessage}
          />
        </>
      )}
    </>
  );
};

export default UnityPlayer;
