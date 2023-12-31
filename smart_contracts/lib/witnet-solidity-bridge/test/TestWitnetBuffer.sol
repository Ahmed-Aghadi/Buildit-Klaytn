// SPDX-License-Identifier: MIT

pragma solidity >=0.7.0 <0.9.0;
pragma experimental ABIEncoderV2;

import "truffle/Assert.sol";
import "../contracts/libs/WitnetBuffer.sol";


contract TestWitnetBuffer {

  using WitnetBuffer for Witnet.Buffer;

  event Log(string _topic, uint256 _value);

  function testRead31bytes() external {
    bytes memory data = hex"58207eadcf3ba9a9a860b4421ee18caa6dca4738fef266aa7b3668a2ff97304cfcab";
    Witnet.Buffer memory buf = Witnet.Buffer(data, 1);
    bytes memory actual = buf.read(31);
    
    Assert.equal(31, actual.length, "Read 31 bytes from a Buffer");
  }

  function testRead32bytes() external {
    bytes memory data = hex"58207eadcf3ba9a9a860b4421ee18caa6dca4738fef266aa7b3668a2ff97304cfcab";
    Witnet.Buffer memory buf = Witnet.Buffer(data, 1);
    bytes memory actual = buf.read(32);
    
    Assert.equal(32, actual.length, "Read 32 bytes from a Buffer");
  }

  function testRead33bytes() external {
    bytes memory data = hex"58207eadcf3ba9a9a860b4421ee18caa6dca4738fef266aa7b3668a2ff97304cfcabff";
    Witnet.Buffer memory buf = Witnet.Buffer(data, 1);
    bytes memory actual = buf.read(33);
    
    Assert.equal(33, actual.length, "Read 33 bytes from a Buffer");
  }

  function testReadUint8() external {
    uint8 expected = 31;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    uint8 actual = buf.readUint8();

    Assert.equal(uint(actual), uint(expected), "Read Uint8 from a Buffer");
  }

  function testReadUint16() external {
    uint16 expected = 31415;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    uint16 actual = buf.readUint16();
    Assert.equal(uint(actual), uint(expected), "Read Uint16 from a Buffer");
  }

  function testReadUint32() external {
    uint32 expected = 3141592653;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    uint32 actual = buf.readUint32();
    Assert.equal(uint(actual), uint(expected), "Read Uint32 from a Buffer");
  }

  function testReadUint64() external {
    uint64 expected = 3141592653589793238;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    uint64 actual = buf.readUint64();
    Assert.equal(uint(actual), uint(expected), "Read Uint64 from a Buffer");
  }

  function testReadUint128() external {
    uint128 expected = 314159265358979323846264338327950288419;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    uint128 actual = buf.readUint128();
    Assert.equal(uint(actual), uint(expected), "Read Uint128 from a Buffer");
  }

  function testReadUint256() external {
    uint256 expected = 31415926535897932384626433832795028841971693993751058209749445923078164062862;
    bytes memory data = abi.encodePacked(expected);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    uint256 actual = buf.readUint256();
    Assert.equal(uint(actual), uint(expected), "Read Uint64 from a Buffer");
  }

  function testMultipleReadHead() external {
    uint8 small = 31;
    uint64 big = 3141592653589793238;
    bytes memory data = abi.encodePacked(small, big);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);

    buf.readUint8();
    uint64 actualBig = buf.readUint64();
    Assert.equal(uint(actualBig), uint(big), "Read multiple data chunks from the same Buffer (inner cursor works as expected)");
  }

  function testReadUint8asUint16() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint8 input = 0xAA;
    bytes memory data = abi.encodePacked(input);

    TestWitnetBuffer(address(throwProxy)).errorReadAsUint16(data);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Reading uint8 as uint16 should fail");
  }

  function testReadUint16asUint32() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint16 input = 0xAAAA;
    bytes memory data = abi.encodePacked(input);

    TestWitnetBuffer(address(throwProxy)).errorReadAsUint32(data);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Reading uint16 as uint32 should fail");
  }

  function testReadUint32asUint64() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint32 input = 0xAAAAAAAA;
    bytes memory data = abi.encodePacked(input);

    TestWitnetBuffer(address(throwProxy)).errorReadAsUint64(data);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Reading uint32 as uint64 should fail");
  }

  function testReadUint64asUint128() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint64 input = 0xAAAAAAAAAAAAAAAA;
    bytes memory data = abi.encodePacked(input);

    TestWitnetBuffer(address(throwProxy)).errorReadAsUint128(data);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Reading uint64 as uint128 should fail");
  }

  function testReadUint128asUint256() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint128 input = 0xAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;
    bytes memory data = abi.encodePacked(input);

    TestWitnetBuffer(address(throwProxy)).errorReadAsUint256(data);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Reading uint128 as uint256 should fail");
  }

  function testNextOutOfBounds() external {
    ThrowProxy throwProxy = new ThrowProxy(address(this));
    bool r;
    bytes memory error;
    uint8 input = 0xAA;
    bytes memory data = abi.encodePacked(input);
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.next();

    TestWitnetBuffer(address(throwProxy)).errorReadNext(buf);
    (r, error) = throwProxy.execute{gas: 100000}();
    Assert.isFalse(r, "Next for out of bounds fail");
  }

  function errorReadAsUint16(bytes memory data) public {
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.readUint16();
  }

  function errorReadAsUint32(bytes memory data) public {
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.readUint32();
  }

  function errorReadAsUint64(bytes memory data) public {
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.readUint64();
  }

  function errorReadAsUint128(bytes memory data) public {
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.readUint128();
  }

  function errorReadAsUint256(bytes memory data) public {
    Witnet.Buffer memory buf = Witnet.Buffer(data, 0);
    buf.readUint256();
  }

  function errorReadNext(Witnet.Buffer memory buf) public {
    buf.next();
  }

}

// Proxy contract for testing throws
contract ThrowProxy {
  address public target;
  bytes internal data;

  constructor (address _target) {
    target = _target;
  }

  // solhint-disable payable-fallback
  fallback () external {    
    data = msg.data;
  }


  function execute() public returns (bool, bytes memory) {
    // solhint-disable-next-line
    return target.call(data);
  }
}
