#pragma once
struct ImDrawCmd;

// Define assertion handler.
#ifndef IM_ASSERT
#include <assert.h>
#define IM_ASSERT(_EXPR)    assert(_EXPR)
#endif

// Lightweight std::vector<> like class to avoid dragging dependencies (also: windows implementation of STL with debug enabled is absurdly slow, so let's bypass it so our code runs fast in debug).
// Our implementation does NOT call C++ constructors/destructors. This is intentional and we do not require it. Do not use this class as a straight std::vector replacement in your code!
template<typename T>
class ImVector
{
public:
    int                         Size;
    int                         Capacity;
    T* Data;

    typedef T                   value_type;
    typedef value_type* iterator;
    typedef const value_type* const_iterator;

    inline ImVector() { Size = Capacity = 0; Data = NULL; }
    inline ~ImVector() { if (Data) ImGui::MemFree(Data); }

    inline bool                 empty() const { return Size == 0; }
    inline int                  size() const { return Size; }
    inline int                  capacity() const { return Capacity; }

    inline value_type& operator[](int i) { IM_ASSERT(i < Size); return Data[i]; }
    inline const value_type& operator[](int i) const { IM_ASSERT(i < Size); return Data[i]; }

    inline void                 clear() { if (Data) { Size = Capacity = 0; ImGui::MemFree(Data); Data = NULL; } }
    inline iterator             begin() { return Data; }
    inline const_iterator       begin() const { return Data; }
    inline iterator             end() { return Data + Size; }
    inline const_iterator       end() const { return Data + Size; }
    inline value_type& front() { IM_ASSERT(Size > 0); return Data[0]; }
    inline const value_type& front() const { IM_ASSERT(Size > 0); return Data[0]; }
    inline value_type& back() { IM_ASSERT(Size > 0); return Data[Size - 1]; }
    inline const value_type& back() const { IM_ASSERT(Size > 0); return Data[Size - 1]; }
    inline void                 swap(ImVector<T>& rhs) { int rhs_size = rhs.Size; rhs.Size = Size; Size = rhs_size; int rhs_cap = rhs.Capacity; rhs.Capacity = Capacity; Capacity = rhs_cap; value_type* rhs_data = rhs.Data; rhs.Data = Data; Data = rhs_data; }

    inline int                  _grow_capacity(int sz) const { int new_capacity = Capacity ? (Capacity + Capacity / 2) : 8; return new_capacity > sz ? new_capacity : sz; }

    inline void                 resize(int new_size) { if (new_size > Capacity) reserve(_grow_capacity(new_size)); Size = new_size; }
    inline void                 resize(int new_size, const T& v) { if (new_size > Capacity) reserve(_grow_capacity(new_size)); if (new_size > Size) for (int n = Size; n < new_size; n++) Data[n] = v; Size = new_size; }
    inline void                 reserve(int new_capacity)
    {
        if (new_capacity <= Capacity)
            return;
        T* new_data = (value_type*)ImGui::MemAlloc((size_t)new_capacity * sizeof(T));
        if (Data)
            memcpy(new_data, Data, (size_t)Size * sizeof(T));
        ImGui::MemFree(Data);
        Data = new_data;
        Capacity = new_capacity;
    }

    inline void                 push_back(const value_type& v) { if (Size == Capacity) reserve(_grow_capacity(Size + 1)); Data[Size++] = v; }
    inline void                 pop_back() { IM_ASSERT(Size > 0); Size--; }
    inline void                 push_front(const value_type& v) { if (Size == 0) push_back(v); else insert(Data, v); }

    inline iterator             erase(const_iterator it) { IM_ASSERT(it >= Data && it < Data + Size); const ptrdiff_t off = it - Data; memmove(Data + off, Data + off + 1, ((size_t)Size - (size_t)off - 1) * sizeof(value_type)); Size--; return Data + off; }
    inline iterator             insert(const_iterator it, const value_type& v) { IM_ASSERT(it >= Data && it <= Data + Size); const ptrdiff_t off = it - Data; if (Size == Capacity) reserve(_grow_capacity(Size + 1)); if (off < (int)Size) memmove(Data + off + 1, Data + off, ((size_t)Size - (size_t)off) * sizeof(value_type)); Data[off] = v; Size++; return Data + off; }
    inline bool                 contains(const value_type& v) const { const T* data = Data;  const T* data_end = Data + Size; while (data < data_end) if (*data++ == v) return true; return false; }
};

#ifndef ImDrawIdx
typedef unsigned short ImDrawIdx;
#endif

struct Vector2
{
    float x;
    float y;
};

struct ImVectorThing
{
    int Size;
    int Capacity;
    void* Data;
};

struct ImVec2
{
    float x, y;
    ImVec2() { x = y = 0.0f; }
    ImVec2(float _x, float _y) { x = _x; y = _y; }
#ifdef IM_VEC2_CLASS_EXTRA          // Define constructor and implicit cast operators in imconfig.h to convert back<>forth from your math types and ImVec2.
    IM_VEC2_CLASS_EXTRA
#endif
};

struct ImDrawVert
{
    ImVec2 pos;
    ImVec2 uv;
    unsigned int col;
};

enum ImDrawListFlags
{
    None = 0,
    AntiAliasedLines = 1 << 0,
    AntiAliasedFill = 1 << 1,
    AllowVtxOffset = 1 << 2,
};

struct ImVec4
{
    float x, y, z, w;
    ImVec4() { x = y = z = w = 0.0f; }
    ImVec4(float _x, float _y, float _z, float _w) { x = _x; y = _y; z = _z; w = _w; }
#ifdef IM_VEC4_CLASS_EXTRA          // Define constructor and implicit cast operators in imconfig.h to convert back<>forth from your math types and ImVec4.
    IM_VEC4_CLASS_EXTRA
#endif
};

typedef void* ImTextureID;

struct ImDrawListSplitter
{
    int _Current;
    int _Count;
    void* _Channels;
};

struct ImDrawList
{
    ImVector<ImDrawCmd> CmdBuffer;
    ImVector<ImDrawIdx> IdxBuffer;
    ImVector<ImDrawVert> VtxBuffer;
    ImDrawListFlags Flags;
    void* _Data;
    char* _OwnerName;
    unsigned int _VtxCurrentOffset;
    unsigned int _VtxCurrentIdx;
    ImDrawVert* _VtxWritePtr;
    unsigned short* _IdxWritePtr;
    ImVector<ImVec4> _ClipRectStack;
    ImVector<ImTextureID> _TextureIdStack;
    ImVector<ImVec2>   _Path;
    ImDrawListSplitter _Splitter;
};

struct ImDrawData
{
    bool Valid;
    ImDrawList** CmdLists;
    int CmdListsCount;
    int TotalIdxCount;
    int TotalVtxCount;
    Vector2 DisplayPos;
    Vector2 DisplaySize;
    Vector2 FramebufferScale;
};

typedef void (*ImDrawCallback)(const ImDrawList* parent_list, const ImDrawCmd* cmd);
// Typically, 1 command = 1 GPU draw call (unless command is a callback)
struct ImDrawCmd
{
    unsigned int    ElemCount;              // Number of indices (multiple of 3) to be rendered as triangles. Vertices are stored in the callee ImDrawList's vtx_buffer[] array, indices in idx_buffer[].
    ImVec4          ClipRect;               // Clipping rectangle (x1, y1, x2, y2)
    ImTextureID     TextureId;              // User-provided texture ID. Set by user in ImfontAtlas::SetTexID() for fonts or passed to Image*() functions. Ignore if never using images or multiple fonts atlas.
    ImDrawCallback  UserCallback;           // If != NULL, call the function instead of rendering the vertices. clip_rect and texture_id will be set normally.
    void* UserCallbackData;       // The draw callback code can access this.

    ImDrawCmd() { ElemCount = 0; ClipRect.x = ClipRect.y = ClipRect.z = ClipRect.w = 0.0f; TextureId = nullptr; UserCallback = nullptr; UserCallbackData = nullptr; }
};