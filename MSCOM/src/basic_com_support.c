#include <windows.h>

#include "utest.h"
#include "dotnet.h"
#include <oaidl.h>

#define OBJECT_PROGID OLESTR("System.Object")


UTEST(DOTNET, PROGID)
{
    // find CLSID
    CLSID CLSID_Object = { 0 };
    HRESULT result = CLSIDFromProgID(OBJECT_PROGID, &CLSID_Object);

    ASSERT_TRUE(SUCCEEDED(result));

    // show CLSID
    OLECHAR *guid_string = NULL;
    result = StringFromCLSID(&CLSID_Object, &guid_string);
    ASSERT_TRUE(SUCCEEDED(result));

    wprintf(OBJECT_PROGID L" has CLSID %ls\n", guid_string);
    // free the guid string
    CoTaskMemFree(guid_string);
    guid_string = NULL;

    // Activate a System.Object
    _Object *object = NULL;
    result = CoCreateInstance(
        &CLSID_Object,
        NULL,
        CLSCTX_ALL,
        &DIID__Object,
        (void**)&object);

    ASSERT_TRUE(SUCCEEDED(result));
    ASSERT_NE(object, NULL);

    // retrieve the DISPID of ToString
    OLECHAR *member_names[] = {
        OLESTR("ToString"),
    };

    DISPID dispIds;
    result = _Object_GetIDsOfNames(
        object, &IID_NULL, member_names, 1, LOCALE_USER_DEFAULT, &dispIds);

    ASSERT_TRUE(SUCCEEDED(result));
    ASSERT_EQ(dispIds, 0L);

    // invoke ToString
    VARIANT var_result; VariantInit(&var_result);
    DISPPARAMS params = { 0 };
    result = _Object_Invoke(
        object,
        dispIds, &IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_PROPERTYGET,
        &params, &var_result, NULL, NULL);

    ASSERT_TRUE(SUCCEEDED(result));

    UTEST_PRINTF("%ls\n", var_result.bstrVal);
    VariantClear(&var_result); // free the variant and the resource within

cleanup:
    _Object_Release(object); // relase the Object
}

UTEST_STATE();

int main(int argc, const char *argv[]) {

    OleInitialize(NULL);

    utest_main(argc, argv);

    return 0;
}
